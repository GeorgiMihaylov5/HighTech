import { NgModule } from "@angular/core";
import { OverviewComponent } from "./overview.component";
import { ProductComponent } from "./components/product/product.component";
import { ProductService } from "../services/product.service";
import { CommonModule } from "@angular/common";
import { HttpClientModule } from "@angular/common/http";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { ToastrModule } from "ngx-toastr";
import { DetailFacade } from "./services/detail-facade.service";
import { DetailGuard } from "./resolvers/detail.resolver";
import { ProductDetailComponent } from "./components/product-detail/product-detail.component";

@NgModule({
  declarations: [
    OverviewComponent,
    ProductComponent,
    ProductDetailComponent
  ],
  imports: [
    CommonModule,
    HttpClientModule,
    BrowserAnimationsModule,
    ToastrModule,
    
  ],
  providers: [
    ProductService,
    DetailGuard,
    DetailFacade
  ]
})
export class OverviewModule { }