import { Component, Input, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { IToken } from 'src/api-authorization/models/token.model';
import { Order, OrderedProduct } from 'src/app/models/order.model';
import { OrderService } from 'src/app/services/order.service';
import { AuthorizeService } from 'src/api-authorization/services/authorize-facade.service';
import * as moment from 'moment';
import { Router } from '@angular/router';

@Component({
  selector: 'app-orders',
  templateUrl: './orders.component.html',
  styleUrls: ['./orders.component.css']
})
export class OrdersComponent implements OnInit {
  @Input() token: Observable<IToken>;
  public orders: Order[];

  constructor(private router: Router,
    private orderApi: OrderService,
    private authService: AuthorizeService) {
  }

  public ngOnInit(): void {
    this.getOrders(this.router.url);
  }

  public getOrderDate(order: Order): string {
    return moment(new Date(order.orderedOn)).format('LLL');
  }

  public getTotalPrice(order: Order) {
    let total = 0;

    order.orderedProducts.forEach((o: OrderedProduct) => {
      total += o.orderedPrice;
    })

    return total;
  }

  private getOrders(url: string) {
    this.token = this.authService.getTokenData();

    if ((url.includes('/manage/(manage:control-panel'))) {
      this.orderApi.getOrders().subscribe(o => {
        this.orders = o;
      })
    }
    else if (url === '/manage/(manage:orders)') {
      this.token.subscribe(data => {
        this.orderApi.getMyOrders(data.nameid).subscribe(o => {
          this.orders = o;
        })
      });
    }
  }
}
