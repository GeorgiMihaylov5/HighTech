import { Component, Input } from '@angular/core';
import { Observable } from 'rxjs';
import { IToken } from 'src/api-authorization/models/token.model';

@Component({
  selector: 'app-orders',
  templateUrl: './orders.component.html',
  styleUrls: ['./orders.component.css']
})
export class OrdersComponent {
  @Input() token: Observable<IToken>;
}
