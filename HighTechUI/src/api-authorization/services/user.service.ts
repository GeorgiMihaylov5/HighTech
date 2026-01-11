import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { LoginRM } from "../models/login-request.model";
import { Inject, Injectable } from "@angular/core";
import { IToken } from "../models/token.model";
import { Observable, OperatorFunction, catchError, map, throwError } from "rxjs";
import { RegisterRM } from "../models/register-request.model";
import { ToastrService } from "ngx-toastr";
import { ErrorService } from "src/app/services/error.service";
import { ApiResponse } from "src/app/models/api-response.model";

@Injectable({
	providedIn: "root"
})
export class UserService {
	constructor(private http: HttpClient,
		@Inject('BASE_URL') private baseUrl: string,
		private errorService: ErrorService) { }

	public login(user: LoginRM): Observable<IToken> {
		return this.http.post<ApiResponse<IToken>>(`${this.baseUrl}Clients/Login`, user)
			.pipe(
				map(response => response.data),
				catchError(this.errorService.handleError.bind(this.errorService))
			);
	}

	public register(user: RegisterRM): Observable<IToken> {
		return this.http.post<ApiResponse<IToken>>(`${this.baseUrl}Clients/Register`, user)
			.pipe(
				map(response => response.data),
				catchError(this.errorService.handleError.bind(this.errorService))
			);
	}
}
