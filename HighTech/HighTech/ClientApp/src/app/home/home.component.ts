import { Component } from '@angular/core';
import { OverviewFacade } from '../overview/services/overview-facade.service';
import { Product } from '../models/product.model';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {
  public products: Product[];
  constructor(private overviewFacade: OverviewFacade) {
    overviewFacade.getMostSellers().subscribe((products: Product[]) => {
      this.products = products;
    });
  }

  public routeToDetail(product: Product) {
    this.overviewFacade.detailProduct(product);
  }
}
