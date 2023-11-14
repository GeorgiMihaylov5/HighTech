import { NgModule } from "@angular/core";
import { OverviewComponent } from "./overview.component";
import { ProductComponent } from "./components/product/product.component";
import { ProductService } from "../services/product.service";
import { CommonModule } from "@angular/common";
import { HttpClientModule } from "@angular/common/http";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { ToastrModule } from "ngx-toastr";

@NgModule({
  declarations: [
    OverviewComponent,
    ProductComponent,
  ],
  imports: [
    CommonModule,
    HttpClientModule,
    BrowserAnimationsModule,
    ToastrModule
  ],
  providers: [
    ProductService
  ]
})
export class OverviewModule { }
