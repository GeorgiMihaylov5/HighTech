import { Component, OnInit } from '@angular/core';
import { ManageServiceFacade } from './services/manage-facade.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-manage',
  templateUrl: './manage.component.html',
  styleUrls: ['./manage.component.css']
})
export class ManageComponent implements OnInit{
  public currentTab: TabType = TabType.Profile;

  constructor(public manageService: ManageServiceFacade,
    private router: Router) {
      
  }
  ngOnInit(): void {
    this.router.navigate(['/manage', { outlets: { 'manage': ['profile'] } }]);
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
