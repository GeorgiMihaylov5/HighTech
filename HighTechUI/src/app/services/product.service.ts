import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { Observable, catchError, map, throwError } from "rxjs";
import { Product } from "../models/product.model";
import { ToastrService } from "ngx-toastr";
import { ErrorService } from "./error.service";
import { ApiResponse } from "../models/api-response.model";

@Injectable()
export class ProductService {
	constructor(private http: HttpClient,
		private errorService: ErrorService,
		@Inject('BASE_URL') private baseUrl: string) {

	}

	public getProducts(): Observable<Product[]> {
		return this.http.get<ApiResponse<Product[]>>((`${this.baseUrl}Products/GetAll`))
			.pipe(
				map(response => response.data),
				catchError(this.errorService.handleError.bind(this.errorService))
			);
	}

	public getMostSellers(): Observable<Product[]> {
		return this.http.get<ApiResponse<Product[]>>((`/Products/GetMostSellers`))
			.pipe(
				map(response => response.data),
				catchError(this.errorService.handleError.bind(this.errorService))
			);
	}

	public createProduct(product: Product): Observable<Product> {
		return this.http.post<ApiResponse<Product>>((`${this.baseUrl}Products/Create`), product)
			.pipe(
				map(response => response.data),
				catchError(this.errorService.handleError.bind(this.errorService))
			);
	}

	public editProduct(product: Product): Observable<Product> {
		return this.http.put<ApiResponse<Product>>((`${this.baseUrl}Products/Edit`), product)
			.pipe(
				map(response => response.data),
				catchError(this.errorService.handleError.bind(this.errorService))
			);
	}

	public deleteProduct(id: string): Observable<boolean> {
		return this.http.delete<ApiResponse<boolean>>((`${this.baseUrl}Products/Delete/${id}`))
			.pipe(
				map(response => response.data),
				catchError(this.errorService.handleError.bind(this.errorService))
			);
	}

	public increaseDiscount(id: string, percentage: number): Observable<Product> {
		return this.http.post<ApiResponse<Product>>((`${this.baseUrl}Products/MakeDiscount`), {
			id: id, percentage: percentage
		})
			.pipe(
				map(response => response.data),
				catchError(this.errorService.handleError.bind(this.errorService))
			);
	}

	public removeDiscount(id: string): Observable<Product> {
		return this.http.post<ApiResponse<Product>>((`${this.baseUrl}Products/RemoveDiscount`), {
			id: id
		})
			.pipe(
				map(response => response.data),
				catchError(this.errorService.handleError.bind(this.errorService))
			);
	}
}