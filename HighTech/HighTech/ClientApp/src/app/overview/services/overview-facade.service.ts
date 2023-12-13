import { BehaviorSubject, Observable, catchError, from, tap } from "rxjs";
import { ProductService } from "../../services/product.service";
import { Product } from "../../models/product.model";
import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { State } from "src/app/core/state.service";
import { OrderService } from "src/app/services/order.service";

@Injectable()
export class OverviewFacade {
    private isActionInProgress: BehaviorSubject<boolean> =
        new BehaviorSubject<boolean>(false);

    constructor(private productService: ProductService,
        private orderApi: OrderService,
        private router: Router,
        private state: State) {
        // const o: Order = {
        //     orderedOn: '12330',
        //     user: {
        //         UserId: '38fa8bb7-f1a7-4396-9e5a-07ca4bd7f6d0',
        //     },
        //     orderedProducts: [
        //         {
        //             id: null,
        //             product: null,
        //             productId: '4cf845ba-dc71-4283-8237-c384dd5d3d47',
        //             orderedPrice: 343,
        //             count: 1
        //         },
        //         {
        //             id: null,
        //             product: null,
        //             productId: '514759a0-7811-4c17-bc0b-c349352872e1',
        //             orderedPrice: 13,
        //             count: 1
        //         }
        //     ]
        // }
        orderApi.getOrders().subscribe(x => console.log(x))
    }

    public get isBusy(): Observable<boolean> {
        return this.isActionInProgress.asObservable();
    }

    public loadProducts(): Observable<Product[]> {
        return this.state.overviewLoaded
            ? this.state.getProduct$()
            : this.productService.getProducts().pipe(
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