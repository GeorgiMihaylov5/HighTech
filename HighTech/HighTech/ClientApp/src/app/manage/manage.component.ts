import { Component, OnInit } from '@angular/core';
import { ManageServiceFacade } from './services/manage-facade.service';
import { NavigationEnd, Router } from '@angular/router';
import { filter } from 'rxjs';

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
    this.router.events
      .pipe(filter((event): event is NavigationEnd => event instanceof NavigationEnd))
      .subscribe((event: NavigationEnd) => {
        const selectedTab = this.getActiveTab(event.url);

        if(selectedTab != null) {
          this.currentTab = selectedTab;
        }
      });
  }

  public ChangeTab(tab: TabType): void {
    this.currentTab = tab;
  }

  private getActiveTab(url: string): TabType {
    console.log(url)
    if (url === '/manage/(manage:control-panel)') {
      return TabType.ControlPanel;
    } else if (url === '/manage/(manage:orders)') {
      return TabType.MyOrders;
    } else if (url === '/manage/(manage:change-password)') {
      return TabType.ChangePassword;
    } else if(url === '/manage') {
      return TabType.Profile;
    }

    return null;
  }
}

export enum TabType {
  Profile = 0,
  MyOrders = 1,
  ChangePassword = 2,
  ControlPanel = 3
}
