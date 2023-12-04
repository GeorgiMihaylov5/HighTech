import { Component, Input } from '@angular/core';
import { Observable } from 'rxjs';
import { IToken } from 'src/api-authorization/models/token.model';

@Component({
  selector: 'app-control-panel',
  templateUrl: './control-panel.component.html',
  styleUrls: ['./control-panel.component.css']
})
export class ControlPanelComponent {
  @Input() token: Observable<IToken>;
  public controlPanelTab: ControlPanelTabType = ControlPanelTabType.Orders;

  public ChangeTab(tab: ControlPanelTabType): void {
    this.controlPanelTab = tab;
  }
}

export enum ControlPanelTabType {
  Orders = 0,
  Clients = 1,
  Employees = 2,
  Products = 3
}

