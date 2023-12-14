import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { Observable, catchError, throwError } from "rxjs";
import { Product } from "../models/product.model";
import { ToastrService } from "ngx-toastr";
import { ErrorService } from "./error.service";

@Injectable()
export class ProductService {
    constructor(private http: HttpClient,
        private errorService: ErrorService,
        @Inject('BASE_URL') private baseUrl: string) {

    }

    public getProducts(): Observable<Product[]> {
        return this.http.get<Product[]>((`${this.baseUrl}Products/GetAll`))
            .pipe(
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }

    public getMostSellers(): Observable<Product[]> {
        return this.http.get<Product[]>((`${this.baseUrl}Products/GetMostSellers`))
            .pipe(
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }

    public createProduct(product: Product): Observable<Product> {
        return this.http.post<Product>((`${this.baseUrl}Products/Create`), product)
            .pipe(
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }

    public editProduct(product: Product): Observable<Product> {
        return this.http.put<Product>((`${this.baseUrl}Products/Edit`), product)
            .pipe(
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }

    public deleteProduct(id: string): Observable<boolean> {
        return this.http.delete<boolean>((`${this.baseUrl}Products/Delete/${id}`))
            .pipe(
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }

    public increaseDiscount(id: string, percentage: number): Observable<Product> {
        return this.http.post<Product>((`${this.baseUrl}Products/MakeDiscount`), { 
            id: id, percentage: percentage 
        })
            .pipe(
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }

    public removeDiscount(id: string): Observable<Product> {
        return this.http.post<Product>((`${this.baseUrl}Products/RemoveDiscount`), { 
            id: id 
        })
            .pipe(
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }
}