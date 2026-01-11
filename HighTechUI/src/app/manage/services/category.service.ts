import { HttpClient } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { Observable, catchError, map, of, switchMap, throwError } from "rxjs";
import { ErrorService } from "src/app/services/error.service";
import { Category } from "src/app/models/category.model";
import { ApiResponse } from "src/app/models/api-response.model";

@Injectable()
export class CategoryService {
	constructor(private http: HttpClient,
		private errorService: ErrorService,
		@Inject('BASE_URL') private baseUrl: string) {

	}

	public getCategories(): Observable<Category[]> {
		return this.http.get<ApiResponse<Category[]>>((`${this.baseUrl}Categories/GetAll`))
			.pipe(
				map(response => response.data),
				catchError(this.errorService.handleError.bind(this.errorService))
			);
	}

	public createCategory(category: Category): Observable<Category> {
		return this.http.post<ApiResponse<Category>>((`${this.baseUrl}Categories/Create`), category)
			.pipe(
				map(response => response.data),
				catchError(this.errorService.handleError.bind(this.errorService))
			);
	}

	public editCategory(category: Category): Observable<Category> {
		return this.http.put<ApiResponse<Category>>((`${this.baseUrl}Categories/Edit`), category)
			.pipe(
				map(response => response.data),
				catchError(this.errorService.handleError.bind(this.errorService))
			);
	}

	public deleteCategory(name: string): Observable<boolean> {
		return this.http.delete<ApiResponse<boolean>>((`${this.baseUrl}Categories/Delete/${name}`))
			.pipe(
				map(response => response.data),
				catchError(this.errorService.handleError.bind(this.errorService))
			);
	}
}