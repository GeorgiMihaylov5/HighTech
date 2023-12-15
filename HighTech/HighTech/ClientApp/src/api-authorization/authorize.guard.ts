import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Observable, combineLatest, of } from 'rxjs';
import { AuthorizeService } from './services/authorize-facade.service';
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
      this.authorize.getTokenData()
    ])
      .pipe(map(data => {
        const isAuthenticated: boolean = data[0];
        const roles: string[] | string = data[1]?.role;

        if (this.handleAuthorization(isAuthenticated, state)) {
          const validRoles: string[] = _next.data.role;

          if (!validRoles) {
            return true;
          }

          if (typeof roles === 'string') {
            return validRoles.includes(roles)
          }
          else if (Array.isArray(roles)) {
            for (let validRole of validRoles) {
              if (roles.includes(validRole)) {
                return true;
              }
            }
          }
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
