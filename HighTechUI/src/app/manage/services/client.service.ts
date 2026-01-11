import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { Observable, catchError, map, of, switchMap, throwError } from "rxjs";
import { Client } from "src/app/manage/models/client.model";
import { ErrorService } from "src/app/services/error.service";
import { IChangePassword } from "../models/change-password.model";
import { ApiResponse } from "src/app/models/api-response.model";

@Injectable()
export class ClientService {
    constructor(private http: HttpClient,
        private errorService: ErrorService,
        @Inject('BASE_URL') private baseUrl: string) {

    }
    
    public getClients(): Observable<Client[]> {
        return this.http.get<ApiResponse<Client[]>>((`${this.baseUrl}Clients/GetAll`))
            .pipe(
                map(response => response.data),
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }

    public getClient(username: string): Observable<Client> {
        return this.http.get<ApiResponse<Client>>((`${this.baseUrl}Clients/GetByUsername?username=${username}`))
            .pipe(
                map(response => response.data),
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }

    public editClient(client: Client): Observable<Client> {
        return this.http.post<ApiResponse<Client>>((`${this.baseUrl}Clients/EditClient`), client)
            .pipe(
                map(response => response.data),
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }

    public changePassword(model: IChangePassword): Observable<void> {
        return this.http.post<ApiResponse<void>>((`${this.baseUrl}Clients/ChangePassword`), model)
            .pipe(
                map((): void => undefined as void),
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }
}