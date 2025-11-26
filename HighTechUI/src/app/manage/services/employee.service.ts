import { HttpClient } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { Observable, catchError } from "rxjs";
import { Client } from "src/app/manage/models/client.model";
import { ErrorService } from "src/app/services/error.service";
import { IEmployee } from "../models/employee.model";
import { IToken } from "src/api-authorization/models/token.model";

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

    public createEmployee(employee: IEmployee): Observable<IEmployee> {
        return this.http.post<IEmployee>((`${this.baseUrl}Employees/CreateEmployee`), employee)
            .pipe(
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }

    public editEmployee(employee: IEmployee): Observable<IEmployee> {
        return this.http.put<IEmployee>((`${this.baseUrl}Employees/EditEmployee`), employee)
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

    public checkUserRole(token: IToken): Observable<boolean> {
        if (typeof token.role === 'string') {
            token.role = [token.role];
        }
        else if (Array.isArray(token.role)) {
            token.role = [...token.role];
        }

        return this.http.post<boolean>(`${this.baseUrl}Employees/CheckUserRole`, token)
            .pipe(
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }
}