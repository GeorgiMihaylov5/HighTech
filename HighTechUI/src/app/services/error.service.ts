import { HttpErrorResponse } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { ToastrService } from "ngx-toastr";
import { throwError } from "rxjs";

@Injectable()
export class ErrorService {

	constructor(private toastr: ToastrService) { }

	handleError(error: HttpErrorResponse) {
		if (error.status === 0) {
			console.error('An error occurred:', error.error);
			this.toastr.error('Network error. Please check your connection.');
		} else {
			this.readError(error.error);
			console.error(
				`Backend returned code ${error.status}, body was: `, error.error);
		}
		return throwError(() => new Error('Something bad happened; please try again later.'));
	}

	private readError(error: any) {
		// Handle array format (e.g., Identity errors)
		if (error instanceof Array) {
			error.forEach(e => {
				this.toastr.error(e.description || e);
			});
		}
		// Handle string error
		else if (typeof error === 'string') {
			this.toastr.error(error);
		}
		// Handle object with message property
		else if (error && error.message) {
			this.toastr.error(error.message);
		}
		// Fallback
		else {
			this.toastr.error('An unexpected error occurred.');
		}
	}
}