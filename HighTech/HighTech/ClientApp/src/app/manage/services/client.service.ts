import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { Observable, catchError, of, switchMap, throwError } from "rxjs";
import { Client } from "src/app/manage/models/client.model";
import { ErrorService } from "src/app/services/error.service";
import { IChangePassword } from "../models/change-password.model";

@Injectable()
export class ClientService {
    constructor(private http: HttpClient,
        private errorService: ErrorService,
        @Inject('BASE_URL') private baseUrl: string) {

    }
    
    public getClients(): Observable<Client[]> {
        return this.http.get<Client[]>((`${this.baseUrl}Clients/GetAll`))
            .pipe(
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }

    public getClient(username: string): Observable<Client> {
        return this.http.get<Client>((`${this.baseUrl}Clients/GetByUsername?username=${username}`))
            .pipe(
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }

    public editClient(client: Client): Observable<Client> {
        return this.http.post<Client>((`${this.baseUrl}Clients/EditClient`), client)
            .pipe(
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }

    public changePassword(model: IChangePassword): Observable<void> {
        return this.http.post((`${this.baseUrl}Clients/ChangePassword`), model)
            .pipe(
                switchMap(_ => of(null)),
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }
}