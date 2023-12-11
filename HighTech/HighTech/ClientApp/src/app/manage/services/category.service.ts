import { HttpClient } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { Observable, catchError, of, switchMap, throwError } from "rxjs";
import { ErrorService } from "src/app/services/error.service";
import { ICategory } from "src/app/models/category.model";

@Injectable()
export class CategoryService {
    constructor(private http: HttpClient,
        private errorService: ErrorService,
        @Inject('BASE_URL') private baseUrl: string) {

    }
    
    public getCategories(): Observable<ICategory[]> {
        return this.http.get<ICategory[]>((`${this.baseUrl}Categories/GetAll`))
            .pipe(
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }

    public createCategory(category: ICategory): Observable<ICategory> {
        return this.http.post<ICategory>((`${this.baseUrl}Categories/Create`), category)
            .pipe(
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }
}