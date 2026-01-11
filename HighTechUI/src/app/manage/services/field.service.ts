import { HttpClient } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { Observable, catchError, map } from "rxjs";
import { Field } from "src/app/models/field.model";
import { ErrorService } from "src/app/services/error.service";
import { ApiResponse } from "src/app/models/api-response.model";

@Injectable()
export class FieldService {
    constructor(private http: HttpClient,
        private errorService: ErrorService,
        @Inject('BASE_URL') private baseUrl: string) {

    }
    
    public getFields(): Observable<Field[]> {
        return this.http.get<ApiResponse<Field[]>>((`${this.baseUrl}Fields/GetFields`))
            .pipe(
                map(response => response.data),
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }

    public createField(field: Field): Observable<Field> {
        return this.http.post<ApiResponse<Field>>((`${this.baseUrl}Fields/Create`), field)
            .pipe(
                map(response => response.data),
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }

    public editField(field: Field): Observable<Field> {
        return this.http.put<ApiResponse<Field>>((`${this.baseUrl}Fields/Edit`), field)
            .pipe(
                map(response => response.data),
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }

    public deleteField(id: string): Observable<boolean> {
        return this.http.delete<ApiResponse<boolean>>((`${this.baseUrl}Fields/Delete/${id}`))
            .pipe(
                map(response => response.data),
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }
}