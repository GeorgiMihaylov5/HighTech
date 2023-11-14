import { BehaviorSubject, Observable, catchError, from, tap } from "rxjs";
import { ProductService } from "../../services/product.service";
import { Product } from "../../models/product.model";
import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { State } from "src/app/core/state.service";

@Injectable()
export class OverviewFacade {
    private isActionInProgress: BehaviorSubject<boolean> =
        new BehaviorSubject<boolean>(false);

    constructor(private productService: ProductService,
        private router: Router,
        private state: State) {

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

    public editProduct(product: Product): Observable<boolean> {
        this.state.selectedProduct = product;
        const navigationPromise = this.router.navigate(["/edit"]);

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