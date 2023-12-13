import { Component, OnInit } from '@angular/core';
import { OrderedProduct } from 'src/app/models/order.model';
import { OverviewFacade } from '../../services/overview-facade.service';
import { State } from 'src/app/core/state.service';
import { Product } from 'src/app/models/product.model';

@Component({
  selector: 'app-product-detail',
  templateUrl: './product-detail.component.html',
  styleUrls: ['./product-detail.component.css']
})
export class ProductDetailComponent implements OnInit {
  public orderedCount: number = 1;
  public product: Product;

  constructor(private overviewService: OverviewFacade,
    private state: State) {
     state.selectedProduct$.subscribe((p => this.product = p))
  }

  public ngOnInit(): void {
  }

  public makeOrder() {
    let a: OrderedProduct = {
      id: null,
      productId: this.product.id,
      product: this.product,
      orderedPrice: this.product.price * this.orderedCount,
      count: this.orderedCount
    }
  }
}
