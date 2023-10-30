import { Component, Inject, Input } from "@angular/core";
import { AuthenticationResultStatus, INavigationState } from "../models/navigation-state.model";
import { BehaviorSubject } from "rxjs";
import { AuthorizeService } from "../authorize.service";
import { ApplicationPaths, QueryParameterNames } from "../api-authorization.constants";
import { NgForm } from "@angular/forms";
import { IApplicationUser } from "../models/user.model";
import { LoginRM } from "../models/login-request.model";
import { HttpClient } from "@angular/common/http";
import { ActivatedRoute, Router } from "@angular/router";
import { User } from "oidc-client";

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  @Input() user: LoginRM;
  public message = new BehaviorSubject<string | null>(null);

  constructor(private http: HttpClient,
    private router: Router,
    private route: ActivatedRoute,
    @Inject('BASE_URL') private baseUrl: string,
    private authService: AuthorizeService) {

  }

  public async login(form: NgForm): Promise<void> {
    this.http.post<IApplicationUser>(this.baseUrl + 'clients/login', form.value).subscribe(async(user: IApplicationUser) => {

      const returnUrl = ''
      const state: INavigationState = { returnUrl };

      const result = await this.authService.signIn(state, user);

      this.message.next(null);
      switch (result.status) {
        case AuthenticationResultStatus.Redirect:
          break;
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

  private async navigateToReturnUrl(returnUrl: string) {

    await this.router.navigateByUrl(returnUrl, {
      replaceUrl: true
    });
  }
}
