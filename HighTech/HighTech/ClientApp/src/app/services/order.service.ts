import { HttpClient } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { ErrorService } from "./error.service";
import { Observable, catchError } from "rxjs";
import { Order } from "../models/order.model";

@Injectable()
export class OrderService {
    constructor(private http: HttpClient,
        private errorService: ErrorService,
        @Inject('BASE_URL') private baseUrl: string) {

    }

    public getOrders(): Observable<Order[]> {
        return this.http.get<Order[]>((`${this.baseUrl}Orders/GetOrders`))
            .pipe(
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }
    public getMyOrders(username: string): Observable<Order[]> {
        return this.http.get<Order[]>((`${this.baseUrl}Orders/GetMyOrders?username=${username}`))
            .pipe(
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }

    public createOrder(order: Order) {
        return this.http.post((`${this.baseUrl}Orders/CreateOrder`), order)
            .pipe(
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }
}