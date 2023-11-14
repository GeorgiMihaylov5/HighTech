import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { Observable, catchError, throwError } from "rxjs";
import { Product } from "../models/product.model";
import { ToastrService } from "ngx-toastr";

@Injectable()
export class ProductService {
    constructor(private http: HttpClient,
        private toastr: ToastrService,
        @Inject('BASE_URL') private baseUrl: string) {
        
    }

    getProducts(): Observable<Product[]> {
        return this.http.get<Product[]>((`${this.baseUrl}Products/GetAll`))
        .pipe(
            catchError(this.handleError.bind(this))
        )
    }

    private handleError(error: HttpErrorResponse) {
        if (error.status === 0) {
          console.error('An error occurred:', error.error);
        } else {
          this.toastr.error(error.error)
          console.error(
            `Backend returned code ${error.status}, body was: `, error.error);
        }
        return throwError(() => new Error('Something bad happened; please try again later.'));
      }
}