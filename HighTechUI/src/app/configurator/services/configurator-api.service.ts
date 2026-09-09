import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable, catchError, of, switchMap, throwError } from 'rxjs';
import { ApiResult } from '../../models/api-result.model';
import { ConfiguratorCategory, ConfiguratorValidationResult } from '../../models/configurator.model';
import { Product } from '../../models/product.model';
import { ErrorService } from '../../services/error.service';

@Injectable()
export class ConfiguratorApiService {
    constructor(
        private http: HttpClient,
        private errorService: ErrorService,
        @Inject('BASE_URL') private baseUrl: string
    ) {}

    public getCategories(): Observable<ConfiguratorCategory[]> {
        return this.http.get<ApiResult<ConfiguratorCategory[]>>(`${this.baseUrl}Configurator/GetCategories`)
            .pipe(
                switchMap(r => r.isSuccess ? of(r.value) : throwError(() => new Error(r.error || 'Operation failed'))),
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }

    public getParts(categoryName: string): Observable<Product[]> {
        return this.http.get<ApiResult<Product[]>>(
            `${this.baseUrl}Configurator/GetParts?categoryName=${encodeURIComponent(categoryName)}`)
            .pipe(
                switchMap(r => r.isSuccess ? of(r.value) : throwError(() => new Error(r.error || 'Operation failed'))),
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }

    public getCompatibleParts(categoryName: string, selection: { [key: string]: string }): Observable<Product[]> {
        return this.http.post<ApiResult<Product[]>>(
            `${this.baseUrl}Configurator/GetCompatibleParts?categoryName=${encodeURIComponent(categoryName)}`,
            { selection })
            .pipe(
                switchMap(r => r.isSuccess ? of(r.value) : throwError(() => new Error(r.error || 'Operation failed'))),
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }

    public validate(selection: { [key: string]: string }): Observable<ConfiguratorValidationResult> {
        return this.http.post<ApiResult<ConfiguratorValidationResult>>(
            `${this.baseUrl}Configurator/Validate`,
            { selection })
            .pipe(
                switchMap(r => r.isSuccess ? of(r.value) : throwError(() => new Error(r.error || 'Operation failed'))),
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }
}
