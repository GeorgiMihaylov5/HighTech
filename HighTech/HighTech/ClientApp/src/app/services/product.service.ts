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

    getProducts(): Observable<Product[]> {
        return this.http.get<Product[]>((`${this.baseUrl}Products/GetAll`))
        .pipe(
            catchError(this.errorService.handleError.bind(this.errorService))
        )
    }
}