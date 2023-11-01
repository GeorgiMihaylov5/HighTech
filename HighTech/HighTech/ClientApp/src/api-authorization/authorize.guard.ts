import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Observable, combineLatest, of } from 'rxjs';
import { AuthorizeService } from './services/authorize.service';
import { map, tap } from 'rxjs/operators';
import { ApplicationPaths, QueryParameterNames } from './api-authorization.constants';

@Injectable({
  providedIn: 'root'
})
export class AuthorizeGuard implements CanActivate {
  constructor(private authorize: AuthorizeService, private router: Router) {
  }
  canActivate(
    _next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {
    return combineLatest([
        this.authorize.isAuthenticated(),
        this.authorize.getAuthUser()
      ])
      .pipe(map(data => {
        const isAuthenticated = data[0];
        const role = data[1]?.role;

        if (this.handleAuthorization(isAuthenticated, state)) {
          return role === _next.data.role as string;
        }

        return false;
      })
      )
  }


  private handleAuthorization(isAuthenticated: boolean, state: RouterStateSnapshot) {
    if (!isAuthenticated) {
      this.router.navigate(ApplicationPaths.LoginPathComponents, {
        queryParams: {
          [QueryParameterNames.ReturnUrl]: state.url
        }
      });

      return false;
    }
    return true;
  }
}
