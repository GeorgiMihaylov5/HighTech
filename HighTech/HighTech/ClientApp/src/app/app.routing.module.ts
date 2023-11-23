import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { OverviewComponent } from './overview/overview.component';
import { ProductDetailComponent } from './overview/components/product-detail/product-detail.component';
import { DetailResolver } from './overview/resolvers/detail.resolver';
import { ProfileComponent } from './manage/components/profile/profile.component';
import { ManageComponent } from './manage/manage.component';
import { HashLocationStrategy, LocationStrategy, PathLocationStrategy } from '@angular/common';



const routes: Routes = [
  { path: '', component: HomeComponent, pathMatch: 'full' },
  { path: 'products', component: OverviewComponent },
  { path: 'detail', component: ProductDetailComponent, resolve: { 'detailFacade': DetailResolver } },
  { path: 'manage/profile', component: ProfileComponent, outlet: "manage" },
  { path: 'manage', component: ManageComponent },

  // { path: 'fetch-data', component: FetchDataComponent, canActivate: [AuthorizeGuard], data: { role: "Administrator"} },
  // { path: 'authenticate', component: AuthNavComponent}
]

@NgModule({
  imports: [
    RouterModule.forRoot(routes, { useHash: true })
  ],
  exports: [RouterModule]
})
export class AppRoutingModule { }
