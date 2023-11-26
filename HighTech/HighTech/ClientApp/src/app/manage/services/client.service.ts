import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { Observable, catchError, of, switchMap, throwError } from "rxjs";
import { IClient } from "src/app/models/client.model";
import { ErrorService } from "src/app/services/error.service";
import { IChangePassword } from "../models/change-password.model";

@Injectable()
export class ClientService {
    constructor(private http: HttpClient,
        private errorService: ErrorService,
        @Inject('BASE_URL') private baseUrl: string) {

    }

    public getClient(username: string): Observable<IClient> {
        return this.http.get<IClient>((`${this.baseUrl}Clients/GetByUsername?username=${username}`))
            .pipe(
                catchError(this.errorService.handleError.bind(this.errorService))
            )
    }

    public editUser(client: IClient): Observable<IClient> {
        return this.http.post<IClient>((`${this.baseUrl}Clients/EditClient`), client)
            .pipe(
                catchError(this.errorService.handleError.bind(this.errorService))
            )
    }

    public changePassword(model: IChangePassword): Observable<void> {
        return this.http.post((`${this.baseUrl}Clients/ChangePassword`), model)
            .pipe(
                switchMap(_ => of(null)),
                catchError(this.errorService.handleError.bind(this.errorService))
            )
    }
}