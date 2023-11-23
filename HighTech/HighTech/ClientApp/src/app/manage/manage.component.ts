import { Component } from '@angular/core';

@Component({
  selector: 'app-manage',
  templateUrl: './manage.component.html',
  styleUrls: ['./manage.component.css']
})
export class ManageComponent {
 public currentTab: TabType = TabType.Profile;

 public ChangeTab(tab: TabType): void {
  this.currentTab = tab;
 }
}

export enum TabType {
  Profile = 0,
  Orders = 1,
  ChangePassword = 2,
  ControlPanel = 3
}
