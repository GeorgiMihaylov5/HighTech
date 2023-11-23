import { EventEmitter, Injectable, Output } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { AuthenticationResultStatus, IAuthenticationResult } from '../models/navigation-state.model';
import { LoginRM } from '../models/login-request.model';
import { AuthUser } from '../models/token-user.model';
import { UserService } from './user.service';
import { RegisterRM } from '../models/register-request.model';
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root'
})
export class AuthorizeService {
  @Output() authorizationChange = new EventEmitter();

  constructor(private userService: UserService,
    private toastrService: ToastrService) { }

  public isAuthenticated(): Observable<boolean> {
    return this.getAccessTokenExpireDate().pipe(
      map((exp: Date | null) => {
        if (exp && exp > new Date()) {
          return true;
        }

        this.removeAccessToken();
        return false;
      })
    )
  }

  public getAccessToken(): Observable<string | null> {
    return of(localStorage.getItem('token'));
  }

  private getAccessTokenExpireDate(): Observable<Date | null> {
    return this.getAuthUser().pipe(
      map((user: AuthUser | null) => {
        if (user == null) {
          return null;
        }

        const expireTimeStamp = user.exp * 1000;
        return new Date(expireTimeStamp);
      })
    )
  }

  public getAuthUser(): Observable<AuthUser | null> {
    return this.getAccessToken().pipe(
      map((token: string | null) => {
        if (token == null) {
          return null;
        }
        
        return JSON.parse(atob(token.split('.')[1])) as AuthUser
      })
    )
  }

  public getUserName(): Observable<string | null> {
    return this.getAuthUser().pipe(
      map((user: AuthUser | null) => {
        if (user == null) {
          return null;
        }
        return user.firstName;
      })
    )
  }

  public setAccessToken(jwtToken: string) {
    localStorage.setItem('token', jwtToken)
  }

  public removeAccessToken(): boolean {
    if (this.getAccessToken()) {
      localStorage.removeItem('token');

      return true;
    }
    return false;
  }

  public async logout(state: any): Promise<IAuthenticationResult> {
    try {
      if (this.removeAccessToken()) {
        this.authorizationChange.emit();
        return this.success(state)
      }

      return this.error("Can't logout!")
    }
    catch {
      return this.error("Can't logout!")
    }
  }

  public login(state: any, user: LoginRM): Observable<IAuthenticationResult> {
    return this.userService.login(user).pipe(
      map((user: AuthUser) => {
        try {
          this.setAccessToken(user.jwt);
          this.authorizationChange.emit();

          return this.success(state);
        }
        catch {
          return this.error("Can't login!");
        }
      })
    )
  }

  public register(state: any, user: RegisterRM): Observable<IAuthenticationResult> {
    try{
      if(user.Password !== user.ConfirmPassword) {
        //TODO
        this.toastrService.error('Passwords dont match!')
        throw Error();
      }

      return this.userService.register(user).pipe(
        map((user: AuthUser) => {
            this.setAccessToken(user.jwt);
            this.authorizationChange.emit();
  
            return this.success(state);
        })
      )
    }
    catch {
      return of(this.error("Can't register!"));
    }
  }

  private error(message: string): IAuthenticationResult {
    return { status: AuthenticationResultStatus.Fail, message };
  }

  private success(state: any): IAuthenticationResult {
    return { status: AuthenticationResultStatus.Success, state };
  }

  private redirect(): IAuthenticationResult {
    return { status: AuthenticationResultStatus.Redirect };
  }
}
