import { HttpClient } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { ErrorService } from "./error.service";
import { Observable, catchError, map } from "rxjs";
import { Order } from "../models/order.model";
import { ApiResponse } from "../models/api-response.model";

@Injectable()
export class OrderService {
	constructor(private http: HttpClient,
		private errorService: ErrorService,
		@Inject('BASE_URL') private baseUrl: string) {

	}

	public getOrders(): Observable<Order[]> {
		return this.http.get<ApiResponse<Order[]>>((`${this.baseUrl}Orders/GetOrders`))
			.pipe(
				map(response => response.data),
				catchError(this.errorService.handleError.bind(this.errorService))
			);
	}
	public getMyOrders(username: string): Observable<Order[]> {
		return this.http.get<ApiResponse<Order[]>>((`${this.baseUrl}Orders/GetMyOrders?username=${username}`))
			.pipe(
				map(response => response.data),
				catchError(this.errorService.handleError.bind(this.errorService))
			);
	}

	public createOrder(order: Order): Observable<void> {
		return this.http.post<ApiResponse<void>>((`${this.baseUrl}Orders/CreateOrder`), order)
			.pipe(
				map((): void => undefined as void),
				catchError(this.errorService.handleError.bind(this.errorService))
			);
	}

	public editStatus(order: Order): Observable<void> {
		return this.http.put<ApiResponse<void>>((`${this.baseUrl}Orders/EditStatus`), order)
			.pipe(
				map((): void => undefined as void),
				catchError(this.errorService.handleError.bind(this.errorService))
			);
	}
}