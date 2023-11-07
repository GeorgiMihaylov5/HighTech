import { Observable } from "rxjs";
import { ProductService } from "./product.service";
import { Product } from "../models/product.model";
import { Injectable } from "@angular/core";

@Injectable()
export class OverviewFacade {
    constructor(private productService: ProductService) {

    }

    public loadProducts(): Observable<Product[]> {
        return this.productService.getProducts();
    }
}