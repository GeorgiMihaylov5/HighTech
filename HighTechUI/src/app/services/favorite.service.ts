import { HttpClient } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { Observable, catchError, of, switchMap, throwError } from "rxjs";
import { ApiResult } from "../models/api-result.model";
import { Favorite, FavoriteAction } from "../models/favorite.model";
import { ErrorService } from "./error.service";

@Injectable()
export class FavoriteService {
	constructor(private http: HttpClient,
		private errorService: ErrorService,
		@Inject('BASE_URL') private baseUrl: string) {

	}

	public getFavorites(username: string): Observable<Favorite[]> {
		return this.http.get<ApiResult<Favorite[]>>((`${this.baseUrl}Favorites/GetMine?username=${encodeURIComponent(username)}`))
			.pipe(
				switchMap(r => r.isSuccess ? of(r.value as Favorite[]) : throwError(() => new Error(r.error || 'Operation failed'))),
				catchError(this.errorService.handleError.bind(this.errorService))
			);
	}

	public getFavoritesCount(username: string): Observable<number> {
		return this.http.get<ApiResult<number>>((`${this.baseUrl}Favorites/GetCount?username=${encodeURIComponent(username)}`))
			.pipe(
				switchMap(r => r.isSuccess ? of(r.value as number) : throwError(() => new Error(r.error || 'Operation failed'))),
				catchError(this.errorService.handleError.bind(this.errorService))
			);
	}

	public isFavorite(productId: string, username: string): Observable<boolean> {
		return this.http.get<ApiResult<boolean>>((`${this.baseUrl}Favorites/IsFavorite?productId=${encodeURIComponent(productId)}&username=${encodeURIComponent(username)}`))
			.pipe(
				switchMap(r => r.isSuccess ? of(r.value as boolean) : throwError(() => new Error(r.error || 'Operation failed'))),
				catchError(this.errorService.handleError.bind(this.errorService))
			);
	}

	public addFavorite(action: FavoriteAction): Observable<Favorite> {
		return this.http.post<ApiResult<Favorite>>((`${this.baseUrl}Favorites/Add`), action)
			.pipe(
				switchMap(r => r.isSuccess ? of(r.value as Favorite) : throwError(() => new Error(r.error || 'Operation failed'))),
				catchError(this.errorService.handleError.bind(this.errorService))
			);
	}

	public deleteFavorite(productId: string, username: string): Observable<boolean> {
		return this.http.delete<ApiResult<boolean>>((`${this.baseUrl}Favorites/Delete?productId=${encodeURIComponent(productId)}&username=${encodeURIComponent(username)}`))
			.pipe(
				switchMap(r => r.isSuccess ? of(r.value as boolean) : throwError(() => new Error(r.error || 'Operation failed'))),
				catchError(this.errorService.handleError.bind(this.errorService))
			);
	}

	public clearFavorites(username: string): Observable<boolean> {
		return this.http.delete<ApiResult<boolean>>((`${this.baseUrl}Favorites/Clear?username=${encodeURIComponent(username)}`))
			.pipe(
				switchMap(r => r.isSuccess ? of(r.value as boolean) : throwError(() => new Error(r.error || 'Operation failed'))),
				catchError(this.errorService.handleError.bind(this.errorService))
			);
	}
}
