import { NgModule } from "@angular/core";
import { OverviewComponent } from "./overview.component";
import { ProductComponent } from "./components/product/product.component";
import { ProductService } from "../services/product.service";
import { CommonModule } from "@angular/common";
import { HttpClientModule } from "@angular/common/http";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { ToastrModule } from "ngx-toastr";
import { ProductDetailComponent } from "./components/product-detail/product-detail.component";
import { AppRoutingModule } from "../app.routing.module";
import { OrderService } from "../services/order.service";
import { FormsModule } from "@angular/forms";
import { OverviewFacade } from "./services/overview-facade.service";
import { BasketComponent } from "../nav-menu/basket/basket.component";

@NgModule({
  declarations: [
    OverviewComponent,
    ProductComponent,
    ProductDetailComponent,
    BasketComponent
  ],
  imports: [
    AppRoutingModule,
    CommonModule,
    HttpClientModule,
    BrowserAnimationsModule,
    ToastrModule,
    FormsModule
  ],
  providers: [
    ProductService,
    OrderService,
    OverviewFacade
  ]
})
export class OverviewModule { }
