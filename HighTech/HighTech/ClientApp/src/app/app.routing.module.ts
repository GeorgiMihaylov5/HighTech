import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { OverviewComponent } from './overview/overview.component';
import { ProductDetailComponent } from './overview/components/product-detail/product-detail.component';

import { ManageComponent } from './manage/manage.component';
import { ProfileComponent } from './manage/components/profile/profile.component';
import { ControlPanelComponent } from './manage/components/control-panel/control-panel.component';
import { OrdersComponent } from './manage/components/orders/orders.component';
import { ChangePasswordComponent } from './manage/components/change-password/change-password.component';
import { ClientsComponent } from './manage/components/control-panel/components/clients/clients.component';
import { EmployeesComponent } from './manage/components/control-panel/components/employees/employees.component';
import { TableComponent } from './manage/components/control-panel/components/table/table.component';
import { CreateComponent } from './manage/components/control-panel/components/create/create.component';
import { BasketComponent } from './nav-menu/basket/basket.component';



const routes: Routes = [
  { path: '', component: HomeComponent, pathMatch: 'full' },
  { path: 'products', component: OverviewComponent },
  { path: 'basket', component: BasketComponent },
  { path: 'detail', component: ProductDetailComponent },
  {
    path: 'manage', component: ManageComponent, children: [
      { path: '', component: ProfileComponent, outlet: 'manage' },
      { path: 'orders', component: OrdersComponent, outlet: 'manage' },
      { path: 'change-password', component: ChangePasswordComponent, outlet: 'manage' },
      {
        path: 'control-panel', component: ControlPanelComponent, outlet: 'manage', children: [
          { path: '', component: OrdersComponent, outlet: 'control-panel' },
          { path: 'clients', component: ClientsComponent, outlet: 'control-panel' },
          { path: 'employees', component: EmployeesComponent, outlet: 'control-panel' },
          { path: 'table', component: TableComponent, outlet: 'control-panel' },
          { path: 'create', component: CreateComponent, outlet: 'control-panel' },
        ]
      },
    ]
  }

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
