import { HttpClient } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { Observable, catchError } from "rxjs";
import { IClient } from "src/app/models/client.model";
import { ErrorService } from "src/app/services/error.service";
import { IEmployee } from "../../models/employee.model";

@Injectable()
export class EmployeeService {
    constructor(private http: HttpClient,
        private errorService: ErrorService,
        @Inject('BASE_URL') private baseUrl: string) {

    }

    public getEmployees(): Observable<IEmployee[]> {
        return this.http.get<IEmployee[]>((`${this.baseUrl}Employees/GetAll`))
            .pipe(
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }

    public getEmployee(username: string): Observable<IEmployee> {
        return this.http.get<IEmployee>((`${this.baseUrl}Employees/GetByUsername?username=${username}`))
            .pipe(
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }

    public editEmployee(employee: IEmployee): Observable<IEmployee> {
        return this.http.post<IEmployee>((`${this.baseUrl}Employees/EditEmployee`), employee)
            .pipe(
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }

    public promote(employee: IEmployee): Observable<IEmployee> {
        return this.http.post<IEmployee>((`${this.baseUrl}Employees/Promote`), employee)
            .pipe(
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }

    public demote(employee: IEmployee): Observable<IEmployee> {
        return this.http.post<IEmployee>((`${this.baseUrl}Employees/Demote`), employee)
            .pipe(
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }
}