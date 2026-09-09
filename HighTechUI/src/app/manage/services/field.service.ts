import { HttpClient } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { Observable, catchError, of, switchMap, throwError } from "rxjs";
import { Field } from "src/app/models/field.model";
import { ErrorService } from "src/app/services/error.service";
import { ApiResult } from "src/app/models/api-result.model";

@Injectable()
export class FieldService {
    constructor(private http: HttpClient,
        private errorService: ErrorService,
        @Inject('BASE_URL') private baseUrl: string) {

    }

    public getFields(): Observable<Field[]> {
        return this.http.get<ApiResult<Field[]>>((`${this.baseUrl}Fields/GetFields`))
            .pipe(
                switchMap(r => r.isSuccess ? of(r.value as Field[]) : throwError(() => new Error(r.error || 'Operation failed'))),
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }

    public createField(field: Field): Observable<Field> {
        return this.http.post<ApiResult<Field>>((`${this.baseUrl}Fields/Create`), field)
            .pipe(
                switchMap(r => r.isSuccess ? of(r.value as Field) : throwError(() => new Error(r.error || 'Operation failed'))),
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }

    public editField(field: Field): Observable<Field> {
        return this.http.put<ApiResult<Field>>((`${this.baseUrl}Fields/Edit`), field)
            .pipe(
                switchMap(r => r.isSuccess ? of(r.value as Field) : throwError(() => new Error(r.error || 'Operation failed'))),
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }

    public deleteField(id: string): Observable<boolean> {
        return this.http.delete<ApiResult<boolean>>((`${this.baseUrl}Fields/Delete/${id}`))
            .pipe(
                switchMap(r => r.isSuccess ? of(r.value as boolean) : throwError(() => new Error(r.error || 'Operation failed'))),
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }
}
