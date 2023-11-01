import { HttpClient } from "@angular/common/http";
import { Component, Inject, Input } from "@angular/core";
import { RegisterRM } from "../../models/register-request.model";
import { NgForm } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { AuthorizeService } from "../../services/authorize.service";
import { AuthenticationResultStatus, INavigationState } from "../../models/navigation-state.model";
import { ApplicationPaths, QueryParameterNames } from "../../api-authorization.constants";
import { BehaviorSubject, tap } from "rxjs";

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  @Input() user: RegisterRM;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private authService: AuthorizeService) { }

  public message = new BehaviorSubject<string | null>(null);

  private async navigateToReturnUrl(returnUrl: string) {
    // It's important that we do a replace here so that we remove the callback uri with the
    // fragment containing the tokens from the browser history.
    await this.router.navigateByUrl(returnUrl, {
      replaceUrl: true
    });
  }

  private getReturnUrl(state?: INavigationState): string {
    const fromQuery = (this.route.snapshot.queryParams as INavigationState).returnUrl;
    // If the url is coming from the query string, check that is either
    // a relative url or an absolute url
    if (fromQuery &&
      !(fromQuery.startsWith(`${window.location.origin}/`) ||
        /\/[^\/].*/.test(fromQuery))) {
      // This is an extra check to prevent open redirects.
      throw new Error('Invalid return url. The return url needs to have the same origin as the current page.');
    }
    return (state && state.returnUrl) ||
      fromQuery ||
      ApplicationPaths.LoggedOut;
  }

  async register(form: NgForm): Promise<void> {
    const returnUrl = this.getReturnUrl()
    const state: INavigationState = { returnUrl };

    try {
      this.authService.register(state, form.value).subscribe(async (result) => {

        this.message.next(null);
        switch (result.status) {
          case AuthenticationResultStatus.Success:
            await this.navigateToReturnUrl(returnUrl);
            break;
          case AuthenticationResultStatus.Fail:
            await this.router.navigate(ApplicationPaths.LoginFailedPathComponents, {
              queryParams: { [QueryParameterNames.Message]: result.message }
            });
            break;
          default:
            throw new Error(`Invalid status result ${(result as any).status}.`);
        }

      })
    }
    catch { 
      await this.router.navigate(ApplicationPaths.LoginFailedPathComponents, {
        queryParams: { [QueryParameterNames.Message]: "Error in register" }
      });
    }
  }
}