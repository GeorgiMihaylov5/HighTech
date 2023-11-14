import { BehaviorSubject, Observable, catchError, from, tap } from "rxjs";
import { ProductService } from "../../services/product.service";
import { Product } from "../../models/product.model";
import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { State } from "src/app/core/state.service";

@Injectable()
export class DetailFacade {
    public product: Product;

    constructor(private productService: ProductService,
        private router: Router,
        private state: State) {

    }

    prepareForDetail(product: Product) {
        this.product = product;
    }
}