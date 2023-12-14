import { Component, Input, OnInit } from '@angular/core';
import { Observable, catchError } from 'rxjs';
import { IToken } from 'src/api-authorization/models/token.model';
import { Order, OrderedProduct, Status } from 'src/app/models/order.model';
import { OrderService } from 'src/app/services/order.service';
import { AuthorizeService } from 'src/api-authorization/services/authorize-facade.service';
import * as moment from 'moment';
import { Router } from '@angular/router';
import { ErrorService } from 'src/app/services/error.service';

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
    private authService: AuthorizeService,
    private errorService: ErrorService) {
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

  public changeStatus(order: Order) {
    this.orderApi.editStatus(order).subscribe(_ => {

    }, catchError(this.errorService.handleError.bind(this.errorService)));
  }

  public statusToString(num: number): string {
    switch (num) {
      case Status.Pending:
        return 'Pending';
      case Status.Approved:
        return 'Approved';
      case Status.Rejected:
        return 'Rejected';
        case Status.Completed:
        return 'Completed';
      default:
        return 'Unknown';
    }
  }

  public getStatuses() {
    return Object.entries(Status)
    .map(([key, value]) => ({ key, value }))
    .filter((v) => isNaN(Number(v.value)))
    .map((v) => ({ key: parseInt(v.key), value: v.value }));
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
