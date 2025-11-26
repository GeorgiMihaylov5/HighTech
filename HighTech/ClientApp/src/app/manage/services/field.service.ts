import { HttpClient } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { Observable, catchError } from "rxjs";
import { Field } from "src/app/models/field.model";
import { ErrorService } from "src/app/services/error.service";

@Injectable()
export class FieldService {
    constructor(private http: HttpClient,
        private errorService: ErrorService,
        @Inject('BASE_URL') private baseUrl: string) {

    }
    
    public getFields(): Observable<Field[]> {
        return this.http.get<Field[]>((`${this.baseUrl}Fields/GetFields`))
            .pipe(
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }

    public createField(field: Field): Observable<Field> {
        return this.http.post<Field>((`${this.baseUrl}Fields/Create`), field)
            .pipe(
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }

    public editField(field: Field): Observable<Field> {
        return this.http.put<Field>((`${this.baseUrl}Fields/Edit`), field)
            .pipe(
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }

    public deleteField(id: string): Observable<boolean> {
        return this.http.delete<boolean>((`${this.baseUrl}Fields/Delete/${id}`))
            .pipe(
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }
}