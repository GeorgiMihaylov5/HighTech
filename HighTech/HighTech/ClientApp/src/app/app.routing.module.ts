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
import { AuthorizeGuard } from 'src/api-authorization/authorize.guard';

const adminRights = { role: ["Administrator"] };
const employeeRights = { role: ["Administrator", "Employee"] };


const routes: Routes = [
  { path: '', component: HomeComponent, pathMatch: 'full' },
  { path: 'products', component: OverviewComponent },
  { path: 'basket', component: BasketComponent },
  { path: 'detail', component: ProductDetailComponent },
  {
    path: 'manage', component: ManageComponent, canActivate: [AuthorizeGuard], children: [
      { path: '', component: ProfileComponent, canActivate: [AuthorizeGuard], outlet: 'manage' },
      { path: 'orders', component: OrdersComponent, canActivate: [AuthorizeGuard], outlet: 'manage' },
      { path: 'change-password', component: ChangePasswordComponent, canActivate: [AuthorizeGuard], outlet: 'manage' },
      {
        path: 'control-panel', component: ControlPanelComponent, outlet: 'manage', canActivate: [AuthorizeGuard], data: employeeRights, children: [
          { path: '', component: OrdersComponent, canActivate: [AuthorizeGuard], data: employeeRights, outlet: 'control-panel' },
          { path: 'clients', component: ClientsComponent, canActivate: [AuthorizeGuard], data: employeeRights, outlet: 'control-panel' },
          { path: 'employees', component: EmployeesComponent, canActivate: [AuthorizeGuard], data: adminRights, outlet: 'control-panel' },
          { path: 'table', component: TableComponent, canActivate: [AuthorizeGuard], data: adminRights, outlet: 'control-panel' },
          { path: 'create', component: CreateComponent, canActivate: [AuthorizeGuard], data: adminRights, outlet: 'control-panel' },
        ]
      },
    ]
  }
]

@NgModule({
  imports: [
    RouterModule.forRoot(routes, { useHash: true })
  ],
  exports: [RouterModule]
})
export class AppRoutingModule { }
