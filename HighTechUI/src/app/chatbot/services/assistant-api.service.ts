import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable, catchError, of, switchMap, throwError } from 'rxjs';
import { ApiResult } from '../../models/api-result.model';
import { ChatRequest, ChatResponse, NewChatRequest } from '../../models/chat.model';
import { ErrorService } from '../../services/error.service';

@Injectable()
export class AssistantApiService {
    constructor(
        private http: HttpClient,
        private errorService: ErrorService,
        @Inject('BASE_URL') private baseUrl: string
    ) {}

    public chat(request: ChatRequest): Observable<ChatResponse> {
        return this.http.post<ApiResult<ChatResponse>>(`${this.baseUrl}Assistant/Chat`, request)
            .pipe(
                switchMap(r => r.isSuccess ? of(r.value) : throwError(() => new Error(r.error || 'Operation failed'))),
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }

    public newChat(request: NewChatRequest): Observable<void> {
        return this.http.post<ApiResult<void>>(`${this.baseUrl}Assistant/NewChat`, request)
            .pipe(
                switchMap(r => r.isSuccess ? of(void 0) : throwError(() => new Error(r.error || 'Operation failed'))),
                catchError(this.errorService.handleError.bind(this.errorService))
            );
    }
}
