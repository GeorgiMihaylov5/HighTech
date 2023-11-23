import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { AuthorizeInterceptor } from 'src/api-authorization/authorize.interceptor';
import { OverviewModule } from './overview/overview.module';
import { CommonModule } from '@angular/common';
import { State } from './core/state.service';
import { ProfileComponent } from './manage/components/profile/profile.component';
import { AppRoutingModule } from './app.routing.module';
import { ManageComponent } from './manage/manage.component';
import { ApiAuthorizationModule } from 'src/api-authorization/api-authorization.module';


@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    ProfileComponent,
    ManageComponent
  ],
  imports: [
    OverviewModule,
    CommonModule,
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    AppRoutingModule,
    ApiAuthorizationModule,
  ],
  providers: [
    State,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthorizeInterceptor,
      multi: true
    },
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
