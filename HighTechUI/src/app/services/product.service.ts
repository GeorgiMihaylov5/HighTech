import { HttpClient } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { Observable, catchError, of, switchMap, throwError } from "rxjs";
import { Product } from "../models/product.model";
import { ApiResult } from "../models/api-result.model";
import { ErrorService } from "./error.service";
import { Review, UpsertReview } from "../models/review.model";

@Injectable()
export class ProductService {
	constructor(private http: HttpClient,
		private errorService: ErrorService,
		@Inject('BASE_URL') private baseUrl: string) {

	}

	public getProducts(): Observable<Product[]> {
		return this.http.get<ApiResult<Product[]>>((`${this.baseUrl}Products/GetAll`))
			.pipe(
				switchMap(r => r.isSuccess ? of(r.value as Product[]) : throwError(() => new Error(r.error || 'Operation failed'))),
				catchError(this.errorService.handleError.bind(this.errorService))
			);
	}

	public getProduct(id: string): Observable<Product> {
		return this.http.get<ApiResult<Product>>((`${this.baseUrl}Products/Get?id=${id}`))
			.pipe(
				switchMap(r => r.isSuccess ? of(r.value as Product) : throwError(() => new Error(r.error || 'Operation failed'))),
				catchError(this.errorService.handleError.bind(this.errorService))
			);
	}

	public getMostSellers(): Observable<Product[]> {
		return this.http.get<ApiResult<Product[]>>((`/Products/GetMostSellers`))
			.pipe(
				switchMap(r => r.isSuccess ? of(r.value as Product[]) : throwError(() => new Error(r.error || 'Operation failed'))),
				catchError(this.errorService.handleError.bind(this.errorService))
			);
	}

	public uploadImage(file: File): Observable<string> {
		const form = new FormData();
		form.append('file', file);

		return this.http.post<ApiResult<string>>((`${this.baseUrl}Products/UploadImage`), form)
			.pipe(
				switchMap(r => r.isSuccess ? of(r.value as string) : throwError(() => new Error(r.error || 'Operation failed'))),
				catchError(this.errorService.handleError.bind(this.errorService))
			);
	}

	public createProduct(product: Product): Observable<Product> {
		return this.http.post<ApiResult<Product>>((`${this.baseUrl}Products/Create`), product)
			.pipe(
				switchMap(r => r.isSuccess ? of(r.value as Product) : throwError(() => new Error(r.error || 'Operation failed'))),
				catchError(this.errorService.handleError.bind(this.errorService))
			);
	}

	public editProduct(product: Product): Observable<Product> {
		return this.http.put<ApiResult<Product>>((`${this.baseUrl}Products/Edit`), product)
			.pipe(
				switchMap(r => r.isSuccess ? of(r.value as Product) : throwError(() => new Error(r.error || 'Operation failed'))),
				catchError(this.errorService.handleError.bind(this.errorService))
			);
	}

	public deleteProduct(id: string): Observable<boolean> {
		return this.http.delete<ApiResult<boolean>>((`${this.baseUrl}Products/Delete/${id}`))
			.pipe(
				switchMap(r => r.isSuccess ? of(r.value as boolean) : throwError(() => new Error(r.error || 'Operation failed'))),
				catchError(this.errorService.handleError.bind(this.errorService))
			);
	}

	public increaseDiscount(id: string, percentage: number): Observable<Product> {
		return this.http.post<ApiResult<Product>>((`${this.baseUrl}Products/MakeDiscount`), {
			id: id, percentage: percentage
		})
			.pipe(
				switchMap(r => r.isSuccess ? of(r.value as Product) : throwError(() => new Error(r.error || 'Operation failed'))),
				catchError(this.errorService.handleError.bind(this.errorService))
			);
	}

	public removeDiscount(id: string): Observable<Product> {
		return this.http.post<ApiResult<Product>>((`${this.baseUrl}Products/RemoveDiscount`), {
			id: id
		})
			.pipe(
				switchMap(r => r.isSuccess ? of(r.value as Product) : throwError(() => new Error(r.error || 'Operation failed'))),
				catchError(this.errorService.handleError.bind(this.errorService))
			);
	}

	public upsertReview(review: UpsertReview): Observable<Review> {
		return this.http.post<ApiResult<Review>>((`${this.baseUrl}Reviews/Upsert`), review)
			.pipe(
				switchMap(r => r.isSuccess ? of(r.value as Review) : throwError(() => new Error(r.error || 'Operation failed'))),
				catchError(this.errorService.handleError.bind(this.errorService))
			);
	}

	public deleteReview(productId: string, username: string): Observable<boolean> {
		return this.http.delete<ApiResult<boolean>>((`${this.baseUrl}Reviews/Delete?productId=${productId}&username=${username}`))
			.pipe(
				switchMap(r => r.isSuccess ? of(r.value as boolean) : throwError(() => new Error(r.error || 'Operation failed'))),
				catchError(this.errorService.handleError.bind(this.errorService))
			);
	}
}
