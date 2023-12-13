import { Component, Input } from '@angular/core';
import { Observable } from 'rxjs';
import { IToken } from 'src/api-authorization/models/token.model';
import { Order, OrderedProduct } from 'src/app/models/order.model';
import { OrderService } from 'src/app/services/order.service';
import { ManageServiceFacade } from '../../services/manage-facade.service';
import { AuthorizeService } from 'src/api-authorization/services/authorize-facade.service';

@Component({
  selector: 'app-orders',
  templateUrl: './orders.component.html',
  styleUrls: ['./orders.component.css']
})
export class OrdersComponent {
  @Input() token: Observable<IToken>;
  public orders: Order[];

  constructor(private orderApi: OrderService,
    private authService: AuthorizeService) {

this.token= authService.getTokenData();

      this.token.subscribe(data => {
        orderApi.getMyOrders(data.nameid).subscribe(o => {
          this.orders = o;
        })
      })
  }

  public getTotalPrice(order: Order) {
    let total = 0;

    order.orderedProducts.forEach((o: OrderedProduct) => {
      total += o.orderedPrice;
    })

    return total;
  }
}
