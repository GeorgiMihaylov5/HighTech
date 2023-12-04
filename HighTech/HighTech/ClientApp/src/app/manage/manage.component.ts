import { Component } from '@angular/core';
import { Observable, map, switchMap } from 'rxjs';
import { IToken } from 'src/api-authorization/models/token.model';
import { AuthorizeService } from 'src/api-authorization/services/authorize-facade.service';

@Component({
  selector: 'app-manage',
  templateUrl: './manage.component.html',
  styleUrls: ['./manage.component.css']
})
export class ManageComponent {
  public currentTab: TabType = TabType.Profile;
  public token: Observable<IToken>;


  constructor(authService: AuthorizeService) {
    this.token = authService.getTokenData();
  }

  public ChangeTab(tab: TabType): void {
    this.currentTab = tab;
  }
}

export enum TabType {
  Profile = 0,
  MyOrders = 1,
  ChangePassword = 2,
  ControlPanel = 3
}
