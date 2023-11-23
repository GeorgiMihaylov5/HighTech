import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthNavComponent } from './components/auth-nav/auth-nav.component';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { LogoutComponent } from './components/logout/logout.component';
import { RouterModule, Routes } from '@angular/router';
import { ApplicationPaths } from './api-authorization.constants';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { ProfileComponent } from 'src/app/manage/components/profile/profile.component';
import { UserService } from './services/user.service';
import { AuthorizeService } from './services/authorize-facade.service';
import { ToastrModule, ToastrService } from 'ngx-toastr';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ManageComponent } from 'src/app/manage/manage.component';

export const apiRoutes: Routes =       [
  { path: ApplicationPaths.Register, component: RegisterComponent },
  { path: ApplicationPaths.Login, component: LoginComponent },
  { path: ApplicationPaths.LoginFailed, component: LoginComponent },
  { path: ApplicationPaths.LoginCallback, component: LoginComponent },
  { path: ApplicationPaths.LogOut, component: LogoutComponent },
  { path: ApplicationPaths.LoggedOut, component: LogoutComponent },
  { path: ApplicationPaths.LogOutCallback, component: LogoutComponent }
]

@NgModule({
  imports: [
    CommonModule,
    HttpClientModule,
    FormsModule,
    BrowserAnimationsModule,
    ToastrModule.forRoot(),
    RouterModule.forChild(apiRoutes)
  ],
  declarations: [AuthNavComponent, LoginComponent, RegisterComponent, LogoutComponent],
  providers: [UserService, AuthorizeService],
  exports: [AuthNavComponent, LoginComponent, RegisterComponent, LogoutComponent]
})
export class ApiAuthorizationModule { }
