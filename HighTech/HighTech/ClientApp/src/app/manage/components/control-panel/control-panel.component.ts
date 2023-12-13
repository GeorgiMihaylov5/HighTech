import { Component, Input, OnInit } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { filter } from 'rxjs';

@Component({
  selector: 'app-control-panel',
  templateUrl: './control-panel.component.html',
  styleUrls: ['./control-panel.component.css']
})
export class ControlPanelComponent implements OnInit {
  @Input() public controlPanelTab: ControlPanelTabType = ControlPanelTabType.Orders;

  constructor(private router: Router) {

  }

  ngOnInit(): void {
    this.setCurrentTab(this.router.url);

    this.router.events
      .pipe(filter((event): event is NavigationEnd => event instanceof NavigationEnd))
      .subscribe((event: NavigationEnd) => {
        this.setCurrentTab(event.url);
      });
  }

  public ChangeTab(tab: ControlPanelTabType): void {
    this.controlPanelTab = tab;
  }

  private getActiveTab(url: string): ControlPanelTabType {
    if (url === '/manage/(manage:control-panel/(control-panel:clients))') {
      return ControlPanelTabType.Clients;
    } else if (url === '/manage/(manage:control-panel/(control-panel:employees))') {
      return ControlPanelTabType.Employees;
    } else if (url === '/manage/(manage:control-panel/(control-panel:table))') {
      return ControlPanelTabType.Products;
    } else if (url === '/manage/(manage:control-panel/(control-panel:create))') {
      return ControlPanelTabType.Create;
    } else if ((url === '/manage/(manage:control-panel)')) {
      return ControlPanelTabType.Orders;
    }
    return null;

  }

  private setCurrentTab(url: string): void {
    const selectedTab = this.getActiveTab(url);

    if (selectedTab != null) {
      this.controlPanelTab = selectedTab;
    }
  }
}

export enum ControlPanelTabType {
  Orders = 0,
  Clients = 1,
  Employees = 2,
  Products = 3,
  Create = 4
}

