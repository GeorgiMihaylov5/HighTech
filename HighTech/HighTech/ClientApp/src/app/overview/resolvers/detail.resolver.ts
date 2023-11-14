import { State } from "src/app/core/state.service";
import { DetailFacade } from "../services/detail-facade.service";
import { Injectable, inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn, RouterStateSnapshot } from "@angular/router";
import { Observable, ObservableInput, catchError, map, switchMap } from "rxjs";
import { Product } from "src/app/models/product.model";

@Injectable()
export class DetailGuard  {

	constructor(
		private state: State,
		protected detailFacade: DetailFacade
	) {
	}

	resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<DetailFacade> {
		return this.state.selectedProduct$.pipe(
			map((settings: Product) =>
				this.detailFacade.prepareForDetail(settings)
			),
			map(() => this.detailFacade),
			catchError((error): ObservableInput<any> => {
				throw Error('Resolver error');
			})
		);
	}
}


export const DetailResolver: ResolveFn<DetailFacade> = (
	route: ActivatedRouteSnapshot,
	state: RouterStateSnapshot
): Observable<DetailFacade> => {
	return inject(DetailGuard).resolve(route, state);
};
