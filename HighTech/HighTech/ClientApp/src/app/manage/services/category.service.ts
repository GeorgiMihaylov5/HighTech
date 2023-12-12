import { HttpClient } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { Observable, catchError, of, switchMap, throwError } from "rxjs";
import { ErrorService } from "src/app/services/error.service";
import { Category } from "src/app/models/category.model";

@Injectable()
export class CategoryService {
    constructor(private http: HttpClient,
        private errorService: ErrorService,
        @Inject('BASE_URL') private baseUrl: string) {

    }
    
    public getCategories(): Observable<Category[]> {
        return this.http.get<Category[]>((`${this.baseUrl}Categories/GetAll`))
            .pipe(
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }

    public createCategory(category: Category): Observable<Category> {
        return this.http.post<Category>((`${this.baseUrl}Categories/Create`), category)
            .pipe(
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }

    public editCategory(category: Category): Observable<Category> {
        return this.http.put<Category>((`${this.baseUrl}Categories/Edit`), category)
            .pipe(
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }

    public deleteCategory(id: string): Observable<boolean> {
        return this.http.delete<boolean>((`${this.baseUrl}Categories/Delete/${id}`))
            .pipe(
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }
}