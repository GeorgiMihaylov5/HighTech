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
}