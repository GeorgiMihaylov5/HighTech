import { NgModule } from "@angular/core";
import { ChangePasswordComponent } from "./components/change-password/change-password.component";
import { ProfileComponent } from "./components/profile/profile.component";
import { ManageComponent } from "./manage.component";
import { ClientService } from "./services/client.service";
import { FormsModule } from "@angular/forms";
import { CommonModule } from "@angular/common";

@NgModule({
  declarations: [
    ChangePasswordComponent,
    ManageComponent,
    ProfileComponent,
    
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
