import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrModule, ToastrService } from 'ngx-toastr';
import { IToken } from 'src/api-authorization/models/token.model';
import { AuthorizeService } from 'src/api-authorization/services/authorize-facade.service';
import { State } from 'src/app/core/state.service';
import { Order, OrderedProduct } from 'src/app/models/order.model';
import { OverviewFacade } from 'src/app/overview/services/overview-facade.service';

@Component({
  selector: 'app-basket',
  templateUrl: './basket.component.html',
  styleUrls: ['./basket.component.css']
})
export class BasketComponent {
  public orders: OrderedProduct[];
  public token: IToken;
  public totalPrice: number = 0;

  constructor(private overviewService: OverviewFacade,
    private router: Router,
    private toastr: ToastrService,
    private authService: AuthorizeService) {
    this.orders = overviewService.getBasket();
    this.calculateTotalPrice();

    authService.getTokenData().subscribe((t: IToken) => {
      this.token = t;
    });
  }
  
  public createOrder() {
    if(this.token == null) {
      this.router.navigateByUrl('authentication/login');

      return;
    }

    if(this.orders ==  null || this.orders?.length === 0) {
      this.toastr.info('The shopping cart is empy!');
      return;
    }

    const order: Order ={
      id: null,
      orderedOn: new Date().getTime().toString(),
      user: null,
      username: this.token.nameid,
      status: 0,
      notes: null,
      orderedProducts: this.orders
    };

    this.overviewService.createOrder(order).subscribe(_ => {
      this.clean();

      this.toastr.success('The order was successful!');
      
    });
  }

  public clean() {
    this.orders = this.overviewService.cleanBasket();
    this.calculateTotalPrice();
  }

  public removeFromBasket(index: number) {
    this.orders = this.overviewService.removeFromBasket(index);
    this.calculateTotalPrice();
  }

  public calculateTotalPrice(): void {
    console.log(this.orders)
    this.totalPrice = 0;

    if(this.orders != null && this.orders.length > 0) {
      this.orders.forEach((o: OrderedProduct) => {
        this.totalPrice += o.product.price * o.count;
      });
    }
  }
}
