import { BehaviorSubject, Observable, catchError, from, tap } from "rxjs";
import { ProductService } from "../../services/product.service";
import { Product } from "../../models/product.model";
import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { State } from "src/app/core/state.service";
import { OrderService } from "src/app/services/order.service";
import { Order, OrderedProduct } from "src/app/models/order.model";

@Injectable()
export class OverviewFacade {
    private isActionInProgress: BehaviorSubject<boolean> =
        new BehaviorSubject<boolean>(false);

    constructor(private productApi: ProductService,
        private orderApi: OrderService,
        private router: Router,
        private state: State) {
    }

    public get isBusy(): Observable<boolean> {
        return this.isActionInProgress.asObservable();
    }

    public loadProducts(): Observable<Product[]> {
        return this.state.overviewLoaded
            ? this.state.getProduct$()
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
        return this.state.getProduct$();
    }

    public detailProduct(product: Product): Observable<boolean> {
        this.state.selectedProduct = product;
        const navigationPromise = this.router.navigate(["/detail"]);

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

    public createOrder(order: Order): Observable<void> {
        return this.orderApi.createOrder(order);
    }

    public addToBasket(orderProduct: OrderedProduct): void {
        const orders = this.getBasket();
        if (orders != null && orders.length > 0) {
            sessionStorage.setItem('basket', JSON.stringify([orderProduct, ...orders]));
        }
        else {
            sessionStorage.setItem('basket', JSON.stringify([orderProduct]));
        }
    }

    public getBasket(): OrderedProduct[] {
        const json = sessionStorage.getItem('basket');

        if (json === undefined) {
            return [];
        }

        return JSON.parse(json);
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
            sessionStorage.setItem('basket', JSON.stringify(orders));
        }

        return orders;
    }

    public cleanBasket(): OrderedProduct[] {
        sessionStorage.removeItem('basket');

        return null;
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
