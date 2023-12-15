import { Component, OnInit } from '@angular/core';
import { ManageServiceFacade } from './services/manage-facade.service';
import { NavigationEnd, Router } from '@angular/router';
import { filter } from 'rxjs';
import { IToken } from 'src/api-authorization/models/token.model';

@Component({
  selector: 'app-manage',
  templateUrl: './manage.component.html',
  styleUrls: ['./manage.component.css']
})
export class ManageComponent implements OnInit {
  public currentTab: TabType = TabType.Profile;
  public showControlPanel: boolean = false;

  constructor(public manageService: ManageServiceFacade,
    private router: Router) {

  }

  ngOnInit(): void {
    this.manageService.checkUserRole().subscribe()

    this.setCurrentTab(this.router.url);

    this.router.events
      .pipe(filter((event): event is NavigationEnd => event instanceof NavigationEnd))
      .subscribe((event: NavigationEnd) => {
        this.setCurrentTab(event.url);
      });

    this.manageService.token.subscribe((token: IToken) => {
      if (token.role.includes('Administrator') || token.role.includes('Employee')) {
        this.showControlPanel = true;
      }
    });
  }

  public ChangeTab(tab: TabType): void {
    this.currentTab = tab;
  }

  private getActiveTab(url: string): TabType {
    if (url.includes('/manage/(manage:control-panel')) {
      return TabType.ControlPanel;
    } else if (url === '/manage/(manage:orders)') {
      return TabType.MyOrders;
    } else if (url === '/manage/(manage:change-password)') {
      return TabType.ChangePassword;
    } else if (url === '/manage') {
      return TabType.Profile;
    }

    return null;
  }

  private setCurrentTab(url: string): void {
    const selectedTab = this.getActiveTab(url);

    if (selectedTab != null) {
      this.currentTab = selectedTab;
    }
  }
}

export enum TabType {
  Profile = 0,
  MyOrders = 1,
  ChangePassword = 2,
  ControlPanel = 3
}
