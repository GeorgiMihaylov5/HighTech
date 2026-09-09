import { BehaviorSubject, Observable, catchError, forkJoin, from, map, of, switchMap, take, tap } from "rxjs";
import { ProductService } from "../../services/product.service";
import { Product } from "../../models/product.model";
import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { State } from "src/app/core/state.service";
import { OrderService } from "src/app/services/order.service";
import { Order, OrderedProduct } from "src/app/models/order.model";
import { Review, UpsertReview } from "src/app/models/review.model";
import { AuthorizeService } from "src/api-authorization/services/authorize-facade.service";
import { IToken } from "src/api-authorization/models/token.model";
import { Favorite } from "src/app/models/favorite.model";
import { FavoriteService } from "src/app/services/favorite.service";
import { ClientService } from "src/app/manage/services/client.service";
import { EmployeeService } from "src/app/manage/services/employee.service";
import { getStoredToken } from "src/app/core/token.util";
import { GroupedBasket, groupOrderedProducts } from "src/app/core/basket-group.util";

export interface CheckoutProfileDefaults {
    phoneNumber: string;
    deliveryAddress: string;
}

export interface ComparisonLoadResult {
    products: Product[];
    removedIds: string[];
}

interface ComparisonProductResult {
    product: Product | null;
    removedId: string | null;
}

@Injectable()
export class OverviewFacade {
    private readonly comparisonStorageKey = 'compare:productIds';
    private readonly comparisonLimit = 3;
    private isActionInProgress: BehaviorSubject<boolean> =
        new BehaviorSubject<boolean>(false);
    private basketItemCountSubject: BehaviorSubject<number> =
        new BehaviorSubject<number>(0);
    private favoriteItemCountSubject: BehaviorSubject<number> =
        new BehaviorSubject<number>(0);
    private comparisonCountSubject: BehaviorSubject<number> =
        new BehaviorSubject<number>(0);

    constructor(private productApi: ProductService,
        private favoriteApi: FavoriteService,
        private orderApi: OrderService,
        private clientApi: ClientService,
        private employeeApi: EmployeeService,
        private router: Router,
        private state: State,
        private authorizeService: AuthorizeService) {
        this.basketItemCountSubject.next(this.getBasketItemCount());
        this.refreshFavoritesCount();
        this.authorizeService.authorizationChange.subscribe(() => {
            this.basketItemCountSubject.next(this.getBasketItemCount());
            this.refreshFavoritesCount();
        });
        this.comparisonCountSubject.next(this.getComparisonIds().length);
    }

    public get isBusy(): Observable<boolean> {
        return this.isActionInProgress.asObservable();
    }

    public get basketItemCount$(): Observable<number> {
        return this.basketItemCountSubject.asObservable();
    }

    public get favoriteItemCount$(): Observable<number> {
        return this.favoriteItemCountSubject.asObservable();
    }

    public get comparisonCount$(): Observable<number> {
        return this.comparisonCountSubject.asObservable();
    }

    public loadProducts(): Observable<Product[]> {
        return this.state.overviewLoaded
            ? this.state.getProducts$()
            : this.productApi.getProducts().pipe(
                tap(
                    (products: Product[]) => {
                        this.state.setProducts$(products);
                        this.state.overviewLoaded = true;
                    }
                )
            );
    }

    public getProducts$(): Observable<Product[]> {
        return this.state.getProducts$();
    }

    public getProduct(id: string): Observable<Product> {
        return this.productApi.getProduct(id).pipe(
            tap((product: Product) => this.syncProductState(product))
        );
    }

    public detailProduct(product: Product): Observable<boolean> {
        this.state.selectedProduct = product;
        const navigationPromise = this.router.navigate(["/detail", product.id]);

        return this.invokeAsyncAction(
            from(navigationPromise)
                .pipe(
                    catchError((error): any => {
                        console.log('Error')
                    }))
        );
    }

    public getMostSellers(): Observable<Product[]> {
        return this.productApi.getMostSellers();
    }

    public upsertReview(review: UpsertReview): Observable<Review> {
        return this.productApi.upsertReview(review);
    }

    public deleteReview(productId: string, username: string): Observable<boolean> {
        return this.productApi.deleteReview(productId, username);
    }

    public getFavorites(): Observable<Favorite[]> {
        const username = this.getCurrentUsername();

        if (username == null) {
            return of([]);
        }

        return this.invokeAsyncAction(this.favoriteApi.getFavorites(username));
    }

    public isFavorite(productId: string): Observable<boolean> {
        const username = this.getCurrentUsername();

        if (username == null) {
            return of(false);
        }

        return this.favoriteApi.isFavorite(productId, username);
    }

    public addFavorite(productId: string): Observable<Favorite> {
        const username = this.getCurrentUsername();

        if (username == null) {
            return of(null);
        }

        return this.invokeAsyncAction(this.favoriteApi.addFavorite({ productId, username }).pipe(
            tap(() => this.refreshFavoritesCount())
        ));
    }

    public deleteFavorite(productId: string): Observable<boolean> {
        const username = this.getCurrentUsername();

        if (username == null) {
            return of(false);
        }

        return this.invokeAsyncAction(this.favoriteApi.deleteFavorite(productId, username).pipe(
            tap(() => this.refreshFavoritesCount())
        ));
    }

    public clearFavorites(): Observable<boolean> {
        const username = this.getCurrentUsername();

        if (username == null) {
            return of(false);
        }

        return this.invokeAsyncAction(this.favoriteApi.clearFavorites(username).pipe(
            tap(() => this.favoriteItemCountSubject.next(0))
        ));
    }

    public getComparisonIds(): string[] {
        const json = sessionStorage.getItem(this.comparisonStorageKey);

        if (json == null) {
            return [];
        }

        try {
            const ids = JSON.parse(json);

            if (!Array.isArray(ids)) {
                return [];
            }

            return ids
                .filter((id: unknown): id is string => typeof id === 'string' && id.trim() !== '')
                .filter((id: string, index: number, allIds: string[]) => allIds.indexOf(id) === index)
                .slice(0, this.comparisonLimit);
        } catch {
            return [];
        }
    }

    public isCompared(productId: string): boolean {
        if (productId == null || productId === '') {
            return false;
        }

        return this.getComparisonIds().includes(productId);
    }

    public toggleCompare(product: Product): boolean | null {
        if (product?.id == null || product.id === '') {
            return false;
        }

        const selectedIds = this.getComparisonIds();

        if (selectedIds.includes(product.id)) {
            this.persistComparisonIds(selectedIds.filter((id: string) => id !== product.id));
            return false;
        }

        if (selectedIds.length >= this.comparisonLimit) {
            return null;
        }

        this.persistComparisonIds([...selectedIds, product.id]);
        return true;
    }

    public removeCompare(productId: string): void {
        if (productId == null || productId === '') {
            return;
        }

        this.persistComparisonIds(this.getComparisonIds().filter((id: string) => id !== productId));
    }

    public clearCompare(): void {
        sessionStorage.removeItem(this.comparisonStorageKey);
        this.comparisonCountSubject.next(0);
    }

    public loadFreshComparisonProducts(): Observable<ComparisonLoadResult> {
        const selectedIds = this.getComparisonIds();

        if (selectedIds.length === 0) {
            return of({ products: [], removedIds: [] });
        }

        const productRequests = selectedIds.map((id: string) =>
            this.productApi.getProduct(id).pipe(
                tap((product: Product) => this.syncProductState(product)),
                map((product: Product): ComparisonProductResult => ({ product, removedId: null })),
                catchError(() => of({ product: null, removedId: id } as ComparisonProductResult))
            )
        );

        return forkJoin(productRequests).pipe(
            map((results) => {
                const products = results
                    .map(result => result.product)
                    .filter((product: Product | null): product is Product => product != null);
                const removedIds = results
                    .map(result => result.removedId)
                    .filter((id: string | null): id is string => id != null);

                if (removedIds.length > 0) {
                    this.persistComparisonIds(selectedIds.filter((id: string) => !removedIds.includes(id)));
                }

                return { products, removedIds };
            })
        );
    }

    public syncProductState(product: Product): void {
        if (product == null) {
            return;
        }

        this.state.selectedProduct = product;
        this.state.updateProduct(product);
    }

    public createOrder(order: Order): Observable<void> {
        return this.orderApi.createOrder(order).pipe(
            switchMap(() => {
                if (!this.state.overviewLoaded) {
                    return of(void 0);
                }

                return this.state.getProducts$().pipe(
                    take(1),
                    tap((products: Product[]) => {
                        const orderedCountsByProductId = order.orderedProducts
                            .reduce((counts: Map<string, number>, op: OrderedProduct) => {
                                const productId = op.productId ?? op.product?.id;

                                if (productId != null) {
                                    counts.set(productId, (counts.get(productId) ?? 0) + (Number(op.count) || 0));
                                }

                                return counts;
                            }, new Map<string, number>());

                        const updatedProducts = products.map((product: Product) => {
                            const orderedCount = orderedCountsByProductId.get(product.id) ?? 0;

                            if (orderedCount <= 0) {
                                return product;
                            }

                            return {
                                ...product,
                                quantity: Math.max(0, product.quantity - orderedCount)
                            };
                        });

                        this.state.setProducts$(updatedProducts);
                    }),
                    map((): void => undefined)
                );
            })
        );
    }

    public getCheckoutProfileDefaults(): Observable<CheckoutProfileDefaults> {
        const token = this.getCurrentToken();

        if (token == null) {
            return of({ phoneNumber: '', deliveryAddress: '' });
        }

        const roles = Array.isArray(token.role) ? token.role : [token.role];
        const profileRequest = roles.includes('Employee') || roles.includes('Administrator')
            ? this.employeeApi.getEmployee(token.nameid).pipe(
                map(profile => ({
                    phoneNumber: profile?.phoneNumber ?? '',
                    deliveryAddress: ''
                }))
            )
            : this.clientApi.getClient(token.nameid).pipe(
                map(profile => ({
                    phoneNumber: profile?.phoneNumber ?? '',
                    deliveryAddress: profile?.address ?? ''
                }))
            );

        return profileRequest.pipe(
            catchError(() => of({ phoneNumber: '', deliveryAddress: '' }))
        );
    }

    public addToBasket(orderProduct: OrderedProduct): number {
        if (this.getCurrentUsername() == null) {
            return -1;
        }

        const orders = this.getBasket();

        // Build items are always added as-is; stock is enforced by the backend at checkout
        if (orderProduct.buildId) {
            const count = Math.max(1, Number(orderProduct.count) || 1);
            this.persistBasket([{ ...orderProduct, count }, ...(orders ?? [])]);
            return count;
        }

        const productId = orderProduct.productId ?? orderProduct.product?.id;
        const existingCount = orders
            ?.filter((order: OrderedProduct) => (order.productId ?? order.product?.id) === productId)
            .reduce((total: number, order: OrderedProduct) => total + order.count, 0) ?? 0;
        const requestedCount = Math.max(1, Number(orderProduct.count) || 1);
        const stockCount = Math.max(0, Number(orderProduct.product?.quantity) || 0);
        const remainingCount = Math.max(0, stockCount - existingCount);
        const addedCount = Math.min(requestedCount, remainingCount);

        if (addedCount <= 0) {
            this.persistBasket(orders ?? []);
            return 0;
        }

        this.persistBasket([{ ...orderProduct, count: addedCount }, ...(orders ?? [])]);

        return addedCount;
    }

    public getBasket(): OrderedProduct[] {
        const storageKey = this.getActiveBasketStorageKey();

        if (storageKey == null) {
            return [];
        }

        const json = sessionStorage.getItem(storageKey);

        if (json == null) {
            return [];
        }

        return this.normalizeBasket(JSON.parse(json));
    }

    public updateBasket(orders: OrderedProduct[]): OrderedProduct[] {
        if (orders == null || orders.length === 0) {
            return this.cleanBasket();
        }

        return this.persistBasket(orders);
    }

    public removeFromBasket(index: number): OrderedProduct[] {
        let orders = this.getBasket();

        if (orders == null) {
            return orders;
        }

        if (orders.length == 1){
            orders = this.cleanBasket();
        }
        else if (orders.length > 1) {
            orders.splice(index, 1);
            orders = this.persistBasket(orders);
        }

        this.basketItemCountSubject.next(this.getBasketItemCount());

        return orders;
    }

    public cleanBasket(): OrderedProduct[] {
        const storageKey = this.getActiveBasketStorageKey();

        if (storageKey != null) {
            sessionStorage.removeItem(storageKey);
        }

        this.basketItemCountSubject.next(0);

        return null;
    }

    public getGroupedBasket(): GroupedBasket {
        return groupOrderedProducts(this.getBasket() ?? []);
    }

    public refreshBasket(): Observable<OrderedProduct[]> {
        const basket = this.getBasket();

        if (basket == null || basket.length === 0) {
            return of(basket ?? []);
        }

        const uniqueProductIds = Array.from(new Set(
            basket
                .map((op: OrderedProduct) => op.productId ?? op.product?.id)
                .filter((id): id is string => typeof id === 'string' && id !== '')
        ));

        if (uniqueProductIds.length === 0) {
            return of(basket);
        }

        const requests = uniqueProductIds.map((id: string) =>
            this.productApi.getProduct(id).pipe(
                map((product: Product) => ({ id, product: product ?? null })),
                catchError(() => of({ id, product: null as Product | null }))
            )
        );

        return forkJoin(requests).pipe(
            map(results => {
                const productById = new Map<string, Product | null>();
                results.forEach(r => productById.set(r.id, r.product));

                const refreshed = basket
                    .map((op: OrderedProduct) => {
                        const id = op.productId ?? op.product?.id;

                        if (id == null) {
                            return null;
                        }

                        const fresh = productById.get(id);

                        if (fresh == null) {
                            return null;
                        }

                        return {
                            ...op,
                            productId: id,
                            product: fresh,
                            orderedPrice: fresh.price
                        } as OrderedProduct;
                    })
                    .filter((op: OrderedProduct | null): op is OrderedProduct => op != null);

                return this.persistBasket(refreshed) ?? [];
            })
        );
    }

    public removeBuild(buildId: string): OrderedProduct[] {
        const basket = this.getBasket();

        if (!basket) {
            return null;
        }

        const remaining = basket.filter(op => op.buildId !== buildId);
        return this.persistBasket(remaining);
    }

    private getBasketItemCount(): number {
        const orders = this.getBasket();

        if (orders == null || orders.length === 0) {
            return 0;
        }

        return orders.reduce((total: number, order: OrderedProduct) => total + order.count, 0);
    }

    private persistBasket(orders: OrderedProduct[]): OrderedProduct[] {
        const normalizedOrders = this.normalizeBasket(orders);

        if (normalizedOrders.length === 0) {
            return this.cleanBasket();
        }

        const storageKey = this.getActiveBasketStorageKey();

        if (storageKey == null) {
            return [];
        }

        sessionStorage.setItem(storageKey, JSON.stringify(normalizedOrders));
        this.basketItemCountSubject.next(this.countBasketItems(normalizedOrders));

        return normalizedOrders;
    }

    private normalizeBasket(orders: OrderedProduct[]): OrderedProduct[] {
        if (orders == null || orders.length === 0) {
            return [];
        }

        const mergedOrders = new Map<string, OrderedProduct>();

        orders.forEach((order: OrderedProduct) => {
            const productId = order.productId ?? order.product?.id;

            if (productId == null) {
                return;
            }

            const key = order.buildId ? `build:${order.buildId}:${productId}` : `item:${productId}`;
            const existingOrder = mergedOrders.get(key);
            const normalizedCount = Math.max(1, Number(order.count) || 1);

            if (existingOrder != null) {
                existingOrder.count += normalizedCount;
                return;
            }

            mergedOrders.set(key, {
                ...order,
                productId,
                count: normalizedCount
            });
        });

        return Array.from(mergedOrders.values())
            .map((order: OrderedProduct) => {
                const stockCount = Math.max(0, Number(order.product?.quantity) || 0);

                return {
                    ...order,
                    count: Math.min(order.count, stockCount)
                };
            })
            .filter((order: OrderedProduct) => order.count > 0);
    }

    private getActiveBasketStorageKey(): string | null {
        const username = this.getCurrentUsername();

        if (username == null || username === '') {
            return null;
        }

        return `basket:${username}`;
    }

    private refreshFavoritesCount(): void {
        const username = this.getCurrentUsername();

        if (username == null) {
            this.favoriteItemCountSubject.next(0);
            return;
        }

        this.favoriteApi.getFavoritesCount(username).subscribe({
            next: (count: number) => this.favoriteItemCountSubject.next(count ?? 0),
            error: () => this.favoriteItemCountSubject.next(0)
        });
    }

    private persistComparisonIds(productIds: string[]): void {
        const normalizedIds = productIds
            .filter((id: string) => id != null && id.trim() !== '')
            .filter((id: string, index: number, allIds: string[]) => allIds.indexOf(id) === index)
            .slice(0, this.comparisonLimit);

        if (normalizedIds.length === 0) {
            sessionStorage.removeItem(this.comparisonStorageKey);
        } else {
            sessionStorage.setItem(this.comparisonStorageKey, JSON.stringify(normalizedIds));
        }

        this.comparisonCountSubject.next(normalizedIds.length);
    }

    private getCurrentUsername(): string | null {
        const username = this.getCurrentToken()?.nameid?.trim();

        return username == null || username === '' ? null : username;
    }

    private getCurrentToken(): IToken | null {
        return getStoredToken();
    }

    private countBasketItems(orders: OrderedProduct[]): number {
        if (orders == null || orders.length === 0) {
            return 0;
        }

        return orders.reduce((total: number, order: OrderedProduct) => total + order.count, 0);
    }

    private invokeAsyncAction(action: Observable<any>) {
        this.isActionInProgress.next(true);
        return action.pipe(
            tap(() => this.isActionInProgress.next(false)),
            catchError((err) => {
                this.isActionInProgress.next(false);
                throw err;
            })
        );
    }
}
