import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-control-panel',
  templateUrl: './control-panel.component.html',
  styleUrls: ['./control-panel.component.css']
})
export class ControlPanelComponent implements OnInit{
  public controlPanelTab: ControlPanelTabType = ControlPanelTabType.Orders;

  constructor(private router: Router) {
      
  }
  ngOnInit(): void {
    this.router.navigate([{ outlets: { 'control-panel': ['orders'] } }]);
  }

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

