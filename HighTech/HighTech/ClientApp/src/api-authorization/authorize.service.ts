import { EventEmitter, Injectable, Output } from '@angular/core';
import { User, UserManager } from 'oidc-client';
import { BehaviorSubject, combineLatest, concat, from, Observable, of } from 'rxjs';
import { filter, map, mergeMap, switchMap, take, tap, timestamp } from 'rxjs/operators';
import { ApplicationPaths, ApplicationName } from './api-authorization.constants';
import { AuthenticationResultStatus, IAuthenticationResult, IUser } from './models/navigation-state.model';
import { IApplicationUser } from './models/user.model';
import { ITokenUser } from './models/token-user.model';

@Injectable({
  providedIn: 'root'
})
export class AuthorizeService {
  @Output() authorization: EventEmitter<IApplicationUser> = new EventEmitter<IApplicationUser>();

  public isAuthenticated(): Observable<boolean> {
    return this.getAccessTokenExpireDate().pipe(
      map((exp: Date | null) => {
        if(exp && exp > new Date()){
          return true;
        }
        return false;
      })
    )
  }


  public getAccessToken(): Observable<string | null> {
    return of(localStorage.getItem('token'));
  }

  private getAccessTokenExpireDate(): Observable<Date | null> {
    return this.getUser().pipe(
      map((user: ITokenUser | null) => {
        if (user == null) {
          return null;
        }

        const expireTimeStamp = user.exp * 1000;
        return new Date(expireTimeStamp);
      })
    )
  }

  public getUser(): Observable<ITokenUser | null> {
    return this.getAccessToken().pipe(
      map((token: string | null) => {
        if (token == null) {
          return null;
        }

        return JSON.parse(atob(token.split('.')[1])) as ITokenUser
      })
    )
  }

  public getUserName(): Observable<string | null> {
    return this.getUser().pipe(
      map((user: ITokenUser | null) => {
        if (user == null) {
          return null;
        }
        return user.given_name;
      })
    )
  }

  public setAccessToken(jwtToken: string) {
    localStorage.setItem('token', jwtToken)
  }

  public removeAccessToken(): boolean{
    if (this.getAccessToken()) {
      localStorage.removeItem('token');

      return true;
    }
      return false;
  }







  //OLD
  private popUpDisabled = true;
  private userManager?: UserManager;
  private userSubject: BehaviorSubject<IApplicationUser | null> = new BehaviorSubject<IApplicationUser | null>(null);

 
  // public getUser(): Observable<IApplicationUser | null> {
  //   return concat(
  //     this.userSubject.pipe(take(1), filter(u => !!u)),
  //     this.userSubject.asObservable());
  // }
  private setUser() {
  }

  // public getAccessToken(): Observable<string | null> {
  //   return from(this.ensureUserManagerInitialized())
  //     .pipe(mergeMap(() => from(this.userManager!.getUser())),
  //       map(user => user && user.access_token));
  // }

  // We try to authenticate the user in three different ways:
  // 1) We try to see if we can authenticate the user silently. This happens
  //    when the user is already logged in on the IdP and is done using a hidden iframe
  //    on the client.
  // 2) We try to authenticate the user using a PopUp Window. This might fail if there is a
  //    Pop-Up blocker or the user has disabled PopUps.
  // 3) If the two methods above fail, we redirect the browser to the IdP to perform a traditional
  //    redirect flow.
  public async signIn(state: any, user: IApplicationUser): Promise<IAuthenticationResult> {
    this.userSubject.next(user);
    
    this.setAccessToken(user.jwt);

    this.authorization.emit();
    return this.success(state);

    // try {
    //   user = await this.userManager!.signinSilent(this.createArguments());


    //   this.userSubject.next(user.profile);
    //   return this.success(state);
    // } catch (silentError) {
    //   // User might not be authenticated, fallback to popup authentication
    //   console.log('Silent authentication error: ', silentError);

    //   try {
    //     if (this.popUpDisabled) {
    //       throw new Error('Popup disabled. Change \'authorize.service.ts:AuthorizeService.popupDisabled\' to false to enable it.');
    //     }
    //     user = await this.userManager!.signinPopup(this.createArguments());
    //     this.userSubject.next(user.profile);
    //     return this.success(state);
    //   } catch (popupError: any) {
    //     if (popupError.message === 'Popup window closed') {
    //       // The user explicitly cancelled the login action by closing an opened popup.
    //       return this.error('The user closed the window.');
    //     } else if (!this.popUpDisabled) {
    //       console.log('Popup authentication error: ', popupError);
    //     }

    //     // PopUps might be blocked by the user, fallback to redirect
    //     try {
    //       let a = this.createArguments(state);
    //       await this.userManager!.signinRedirect(a);
    //       return this.redirect();

    //     } catch (redirectError: any) {
    //       console.log('Redirect authentication error: ', redirectError);
    //       return this.error(redirectError);
    //     }
    //   }
    // }
  }

  public async completeSignIn(url: string): Promise<IAuthenticationResult> {
    // try {
    //   await this.ensureUserManagerInitialized();
    //   const user = await this.userManager!.signinCallback(url);
    //   this.userSubject.next(user && user.profile);
    //   return this.success(user && user.state);
    // } catch (error) {
    //   console.log('There was an error signing in: ', error);
      return this.error('There was an error signing in.');
    //}
  }

  public async signOut(state: any): Promise<IAuthenticationResult> {
    // try {
    //   if (this.popUpDisabled) {
    //     throw new Error('Popup disabled. Change \'authorize.service.ts:AuthorizeService.popupDisabled\' to false to enable it.');
    //   }

    //   await this.ensureUserManagerInitialized();
    //   await this.userManager!.signoutPopup(this.createArguments());
    //   this.userSubject.next(null);
    //   return this.success(state);
    // } catch (popupSignOutError: any) {
    //   console.log('Popup signout error: ', popupSignOutError);
    //   try {
    //     
    //     return this.redirect();
    //   } catch (redirectSignOutError: any) {
    //     console.log('Redirect signout error: ', redirectSignOutError);
    //     return this.error(redirectSignOutError);
    //   }
    // }

    if(this.removeAccessToken()){
      this.authorization.emit();
      return this.success(state)
    }
    return this.redirect()
  }

  public async completeSignOut(url: string): Promise<IAuthenticationResult> {
    await this.ensureUserManagerInitialized();
    try {
      const response = await this.userManager!.signoutCallback(url);
      this.userSubject.next(null);
      return this.success(response && response.state);
    } catch (error: any) {
      console.log(`There was an error trying to log out '${error}'.`);
      return this.error(error);
    }
  }

  private createArguments(state?: any): any {
    return { useReplaceToNavigate: true, data: state };
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

  private async ensureUserManagerInitialized(): Promise<void> {
    if (this.userManager !== undefined) {
      return;
    }

    const response = await fetch(ApplicationPaths.ApiAuthorizationClientConfigurationUrl);
    if (!response.ok) {
      throw new Error(`Could not load settings for '${ApplicationName}'`);
    }

    const settings: any = await response.json();
    settings.automaticSilentRenew = true;
    settings.includeIdTokenInSilentRenew = true;
    this.userManager = new UserManager(settings);

    this.userManager.events.addUserSignedOut(async () => {
      await this.userManager!.removeUser();
      this.userSubject.next(null);
    });
  }
}
