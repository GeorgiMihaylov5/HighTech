import { HttpClient } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { ErrorService } from "./error.service";
import { Observable, catchError, of, switchMap, throwError } from "rxjs";
import { Order } from "../models/order.model";
import { ApiResult } from "../models/api-result.model";

@Injectable()
export class OrderService {
    constructor(private http: HttpClient,
        private errorService: ErrorService,
        @Inject('BASE_URL') private baseUrl: string) {

    }

    public getOrders(): Observable<Order[]> {
        return this.http.get<ApiResult<Order[]>>((`${this.baseUrl}Orders/GetOrders`))
            .pipe(
                switchMap(r => r.isSuccess ? of(r.value as Order[]) : throwError(() => new Error(r.error || 'Operation failed'))),
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }

    public getMyOrders(username: string): Observable<Order[]> {
        return this.http.get<ApiResult<Order[]>>((`${this.baseUrl}Orders/GetMyOrders?username=${username}`))
            .pipe(
                switchMap(r => r.isSuccess ? of(r.value as Order[]) : throwError(() => new Error(r.error || 'Operation failed'))),
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }

    public createOrder(order: Order): Observable<void> {
        return this.http.post<ApiResult<boolean>>((`${this.baseUrl}Orders/CreateOrder`), order)
            .pipe(
                switchMap(r => r.isSuccess ? of(void 0) : throwError(() => new Error(r.error || 'Operation failed'))),
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }

    public editStatus(order: Order): Observable<void> {
        return this.http.put<ApiResult<boolean>>((`${this.baseUrl}Orders/EditStatus`), order)
            .pipe(
                switchMap(r => r.isSuccess ? of(void 0) : throwError(() => new Error(r.error || 'Operation failed'))),
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }
}
