import { HttpClient } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { Observable, catchError, of, switchMap, throwError } from "rxjs";
import { ErrorService } from "src/app/services/error.service";
import { IEmployee } from "../models/employee.model";
import { IToken } from "src/api-authorization/models/token.model";
import { ApiResult } from "src/app/models/api-result.model";
import { AdminDashboard } from "../models/admin-dashboard.model";

@Injectable()
export class EmployeeService {
    constructor(private http: HttpClient,
        private errorService: ErrorService,
        @Inject('BASE_URL') private baseUrl: string) {

    }

    public getEmployees(): Observable<IEmployee[]> {
        return this.http.get<ApiResult<IEmployee[]>>((`${this.baseUrl}Employees/GetAll`))
            .pipe(
                switchMap(r => r.isSuccess ? of(r.value as IEmployee[]) : throwError(() => new Error(r.error || 'Operation failed'))),
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }

    public getDashboard(): Observable<AdminDashboard> {
        return this.http.get<ApiResult<AdminDashboard>>((`${this.baseUrl}Employees/GetDashboard`))
            .pipe(
                switchMap(r => r.isSuccess ? of(r.value as AdminDashboard) : throwError(() => new Error(r.error || 'Operation failed'))),
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }

    public getEmployee(username: string): Observable<IEmployee> {
        return this.http.get<ApiResult<IEmployee>>((`${this.baseUrl}Employees/GetByUsername?username=${username}`))
            .pipe(
                switchMap(r => r.isSuccess ? of(r.value as IEmployee) : throwError(() => new Error(r.error || 'Operation failed'))),
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }

    public createEmployee(employee: IEmployee): Observable<IEmployee> {
        return this.http.post<ApiResult<IEmployee>>((`${this.baseUrl}Employees/CreateEmployee`), employee)
            .pipe(
                switchMap(r => r.isSuccess ? of(r.value as IEmployee) : throwError(() => new Error(r.error || 'Operation failed'))),
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }

    public editEmployee(employee: IEmployee): Observable<IEmployee> {
        return this.http.put<ApiResult<IEmployee>>((`${this.baseUrl}Employees/EditEmployee`), employee)
            .pipe(
                switchMap(r => r.isSuccess ? of(r.value as IEmployee) : throwError(() => new Error(r.error || 'Operation failed'))),
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }

    public promote(employee: IEmployee): Observable<void> {
        return this.http.post<ApiResult<boolean>>((`${this.baseUrl}Employees/Promote`), employee)
            .pipe(
                switchMap(r => r.isSuccess ? of(void 0) : throwError(() => new Error(r.error || 'Operation failed'))),
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }

    public demote(employee: IEmployee): Observable<void> {
        return this.http.post<ApiResult<boolean>>((`${this.baseUrl}Employees/Demote`), employee)
            .pipe(
                switchMap(r => r.isSuccess ? of(void 0) : throwError(() => new Error(r.error || 'Operation failed'))),
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

        return this.http.post<ApiResult<boolean>>(`${this.baseUrl}Employees/CheckUserRole`, token)
            .pipe(
                switchMap(r => r.isSuccess ? of(r.value as boolean) : throwError(() => new Error(r.error || 'Operation failed'))),
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }
}
