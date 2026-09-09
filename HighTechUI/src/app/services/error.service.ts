import { HttpErrorResponse } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { ToastrService } from "ngx-toastr";
import { throwError } from "rxjs";
import { ApiResult } from "../models/api-result.model";

@Injectable()
export class ErrorService {
  private readonly apiUnavailableMessage = 'Cannot reach the server. Please make sure the API is running and try again.';

  constructor(private toastr: ToastrService) { }

  handleError(error: HttpErrorResponse) {
    if (error.status === 0) {
      console.error('An error occurred:', error.error);
      this.toastr.error(this.apiUnavailableMessage);
    } else {
      const apiResult = this.tryParseApiResult(error.error);
      if (apiResult && !apiResult.isSuccess) {
        this.readError(apiResult.error || apiResult.errorTitle || 'An unexpected error occurred.');
        console.error(`Backend returned code ${error.status}, error: `, apiResult.error);
      } else {
        this.readError(error.error);
        console.error(`Backend returned code ${error.status}, body was: `, error.error);
      }
    }
    return throwError(() => new Error('Something bad happened; please try again later.'));
  }

  private tryParseApiResult(error: any): ApiResult<any> | null {
    if (error && typeof error === 'object' &&
        'isSuccess' in error &&
        'error' in error &&
        'statusCode' in error) {
      return error as ApiResult<any>;
    }
    return null;
  }

  private readError(error: any) {
    if (error instanceof Array) {
      const messages = error
        .map(e => this.resolveErrorMessage(e))
        .filter((message): message is string => !!message);

      if (messages.length) {
        messages.forEach(message => this.toastr.error(message));
      } else {
        this.toastr.error(this.apiUnavailableMessage);
      }
    }
    else {
      this.toastr.error(this.resolveErrorMessage(error) ?? this.apiUnavailableMessage);
    }
  }

  private resolveErrorMessage(error: any): string | null {
    if (typeof error === 'string') {
      return error.trim() || null;
    }

    if (error && typeof error === 'object') {
      return error.description || error.message || error.title || null;
    }

    return null;
  }
}
