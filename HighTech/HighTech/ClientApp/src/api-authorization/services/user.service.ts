import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { LoginRM } from "../models/login-request.model";
import { Inject, Injectable } from "@angular/core";
import { AuthUser } from "../models/token-user.model";
import { Observable, catchError, throwError } from "rxjs";
import { RegisterRM } from "../models/register-request.model";
import { ToastrService } from "ngx-toastr";

@Injectable({
    providedIn: "root"
})
export class UserService {
    constructor(private http: HttpClient,
        @Inject('BASE_URL') private baseUrl: string,
        private toastr: ToastrService) { }

    public login(user: LoginRM): Observable<AuthUser> {
        return this.http.post<AuthUser>(`${this.baseUrl}clients/login`, user)
        .pipe(
            catchError(this.handleError.bind(this))
        );;
    }

    public register(user: RegisterRM): Observable<AuthUser> {
        return this.http.post<AuthUser>(`${this.baseUrl}clients/register`, user)
        .pipe(
            catchError(this.handleError.bind(this))
        );
    }

    public getUser() {
        //TODO
    }

    private handleError(error: HttpErrorResponse) {
        if (error.status === 0) {
          console.error('An error occurred:', error.error);
        } else {
         console.log(this)
        this.toastr.error(error.error)
          console.error(
            `Backend returned code ${error.status}, body was: `, error.error);
        }
        return throwError(() => new Error('Something bad happened; please try again later.'));
      }
}