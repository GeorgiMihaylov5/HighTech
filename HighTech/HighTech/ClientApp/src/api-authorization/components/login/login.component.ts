import { Component, Inject, Input } from "@angular/core";
import { AuthenticationResultStatus, INavigationState } from "../../models/navigation-state.model";
import { BehaviorSubject } from "rxjs";
import { AuthorizeService } from "../../services/authorize-facade.service";
import { ApplicationPaths, QueryParameterNames } from "../../api-authorization.constants";
import { NgForm } from "@angular/forms";
import { AppUser } from "../../models/user.model";
import { LoginRM } from "../../models/login-request.model";
import { HttpClient } from "@angular/common/http";
import { ActivatedRoute, Router } from "@angular/router";
import { AuthUser } from "../../models/token-user.model";

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
    const returnUrl = ''
    const state: INavigationState = { returnUrl };

    this.authService.login(state, form.value).subscribe(async result => {
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
    });
  }

  private async navigateToReturnUrl(returnUrl: string) {

    await this.router.navigateByUrl(returnUrl, {
      replaceUrl: true
    });
  }
}
