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
import { TableComponent } from "./components/control-panel/components/table/table.component";
import { EmployeeService } from "./services/employee.service";
import { AppRoutingModule } from "../app.routing.module";
import { ManageServiceFacade } from "./services/manage-facade.service";
import { CreateComponent } from "./components/control-panel/components/create/create.component";
import { FieldService } from "./services/field.service";
import { DashboardComponent } from "./components/control-panel/components/dashboard/dashboard.component";

@NgModule({
  declarations: [
    OrdersComponent,
    ChangePasswordComponent,
    ProfileComponent,
    EmployeesComponent,
    ClientsComponent,
    TableComponent,
    CreateComponent,
    ControlPanelComponent,
    ManageComponent,
    DashboardComponent,
  ],
  imports: [
    CommonModule,
    FormsModule, 
    AppRoutingModule
  ],
  providers: [
    ClientService,
    EmployeeService,
    FieldService,
    ManageServiceFacade
  ]
})
export class ManageModule { }
