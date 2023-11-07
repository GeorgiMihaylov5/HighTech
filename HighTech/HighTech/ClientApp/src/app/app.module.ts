import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { ApiAuthorizationModule } from 'src/api-authorization/api-authorization.module';
import { AuthorizeInterceptor } from 'src/api-authorization/authorize.interceptor';
import { OverviewModule } from './overview/overview.module';
import { ProductComponent } from './overview/components/product/product.component';
import { OverviewComponent } from './overview/overview.component';
import { CommonModule } from '@angular/common';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent
  ],
  imports: [
    OverviewModule,
    CommonModule,
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    ApiAuthorizationModule,
    //TODO
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
       { path: 'products', component: OverviewComponent },
      // { path: 'fetch-data', component: FetchDataComponent, canActivate: [AuthorizeGuard], data: { role: "Administrator"} },
      // { path: 'authenticate', component: AuthNavComponent}
    ],  { useHash: true })
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthorizeInterceptor, multi: true }, 
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
