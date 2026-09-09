import { HttpClient, HttpParams } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { Observable, catchError, map, of, switchMap, throwError } from "rxjs";
import { ErrorService } from "src/app/services/error.service";
import { Category } from "src/app/models/category.model";
import { ApiResult } from "src/app/models/api-result.model";
import { CategoryRemovalPreview } from "src/app/models/category-removal-preview.model";

@Injectable()
export class CategoryService {
    constructor(private http: HttpClient,
        private errorService: ErrorService,
        @Inject('BASE_URL') private baseUrl: string) {

    }

    public getCategories(): Observable<Category[]> {
        return this.http.get<ApiResult<Category[]>>((`${this.baseUrl}Categories/GetAll`))
            .pipe(
                switchMap(r => r.isSuccess ? of(r.value as Category[]) : throwError(() => new Error(r.error || 'Operation failed'))),
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }

    public getUniqueCategories(): Observable<Category[]> {
        return this.getCategories().pipe(
            map((categories: Category[]) => {
                const seen = new Set<string>();
                return (categories ?? []).filter(category => {
                    if (category == null || seen.has(category.name)) {
                        return false;
                    }
                    seen.add(category.name);
                    return true;
                });
            })
        );
    }

    public createCategory(category: Category): Observable<Category> {
        return this.http.post<ApiResult<Category>>((`${this.baseUrl}Categories/Create`), category)
            .pipe(
                switchMap(r => r.isSuccess ? of(r.value as Category) : throwError(() => new Error(r.error || 'Operation failed'))),
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }

    public editCategory(category: Category): Observable<Category> {
        return this.http.put<ApiResult<Category>>((`${this.baseUrl}Categories/Edit`), category)
            .pipe(
                switchMap(r => r.isSuccess ? of(r.value as Category) : throwError(() => new Error(r.error || 'Operation failed'))),
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }

    public previewFieldRemoval(id: string, fieldIds: string[]): Observable<CategoryRemovalPreview> {
        let params = new HttpParams().set('id', id);
        fieldIds.forEach(fieldId => {
            params = params.append('fieldIds', fieldId);
        });

        return this.http.get<ApiResult<CategoryRemovalPreview>>((`${this.baseUrl}Categories/PreviewFieldRemoval`), { params })
            .pipe(
                switchMap(r => r.isSuccess ? of(r.value as CategoryRemovalPreview) : throwError(() => new Error(r.error || 'Operation failed'))),
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }

    public previewCategoryRemoval(id: string): Observable<CategoryRemovalPreview> {
        return this.http.get<ApiResult<CategoryRemovalPreview>>((`${this.baseUrl}Categories/PreviewCategoryRemoval?id=${encodeURIComponent(id)}`))
            .pipe(
                switchMap(r => r.isSuccess ? of(r.value as CategoryRemovalPreview) : throwError(() => new Error(r.error || 'Operation failed'))),
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }

    public deleteCategory(id: string): Observable<boolean> {
        return this.http.delete<ApiResult<boolean>>((`${this.baseUrl}Categories/Delete/${id}`))
            .pipe(
                switchMap(r => r.isSuccess ? of(r.value as boolean) : throwError(() => new Error(r.error || 'Operation failed'))),
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }
}
