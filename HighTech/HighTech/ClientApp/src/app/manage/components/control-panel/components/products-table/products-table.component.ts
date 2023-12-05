import { Component } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { ManageServiceFacade } from 'src/app/manage/services/manage-facade.service';
import { Product } from 'src/app/models/product.model';
import { ProductService } from 'src/app/services/product.service';

@Component({
  selector: 'app-products-table',
  templateUrl: './products-table.component.html',
  styleUrls: ['./products-table.component.css']
})
export class ProductsTableComponent {
  public products: Observable<Product[]>;

  constructor(
    public manageService: ManageServiceFacade,
    private productService: ProductService,
    private toastr: ToastrService
  ) {
    this.products = productService.getProducts()
  }
}
