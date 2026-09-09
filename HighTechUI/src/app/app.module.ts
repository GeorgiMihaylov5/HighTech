import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { AuthorizeInterceptor } from 'src/api-authorization/authorize.interceptor';
import { OverviewModule } from './overview/overview.module';
import { CommonModule } from '@angular/common';
import { State } from './core/state.service';
import { AppRoutingModule } from './app.routing.module';
import { ApiAuthorizationModule } from 'src/api-authorization/api-authorization.module';
import { ErrorService } from './services/error.service';
import { ManageModule } from './manage/manage.module';
import { CategoryService } from './services/category.service';
import { ConfiguratorModule } from './configurator/configurator.module';
import { ChatbotModule } from './chatbot/chatbot.module';


@NgModule({
	declarations: [
		AppComponent,
		NavMenuComponent,
		HomeComponent,
	],
	imports: [
		BrowserModule,
		HttpClientModule,
		AppRoutingModule,
		OverviewModule,
		ManageModule,
		ConfiguratorModule,
		ChatbotModule,
		CommonModule,
		ApiAuthorizationModule,
	],
	providers: [
		State,
		{
			provide: HTTP_INTERCEPTORS,
			useClass: AuthorizeInterceptor,
			multi: true
		},
		ErrorService,
		CategoryService
	],
	bootstrap: [AppComponent]
})
export class AppModule { }
