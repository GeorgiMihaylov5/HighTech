import { NgModule } from "@angular/core";
import { ChangePasswordComponent } from "./components/change-password/change-password.component";
import { ProfileComponent } from "./components/profile/profile.component";
import { ManageComponent } from "./manage.component";
import { ClientService } from "./services/client.service";
import { FormsModule } from "@angular/forms";
import { CommonModule } from "@angular/common";
import { ControlPanelComponent } from "./components/control-panel/control-panel.component";
import { OrdersComponent } from "./components/orders/orders.component";
import { EmployeesComponent } from "./components/control-panel/components/employees/employees.component";
import { ClientsComponent } from "./components/control-panel/components/clients/clients.component";
import { ProductsTableComponent } from "./components/control-panel/components/products-table/products-table.component";

@NgModule({
  declarations: [
    OrdersComponent,
    ChangePasswordComponent,
    ProfileComponent,
    EmployeesComponent,
    ClientsComponent,
    ProductsTableComponent,
    ControlPanelComponent,
    ManageComponent,
  ],
  imports: [
    CommonModule,
    FormsModule
  ],
  providers: [
    ClientService,
  ]
})
export class ManageModule { }
