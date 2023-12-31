import { Component, OnInit } from '@angular/core';
import { AuthorizeService } from '../../services/authorize-facade.service';
import { Observable, of } from 'rxjs';

@Component({
  selector: 'app-auth-nav',
  templateUrl: './auth-nav.component.html',
  styleUrls: ['./auth-nav.component.css']
})
export class AuthNavComponent implements OnInit {
  public isAuthenticated?: Observable<boolean>;
  public name: Observable<string | null>;

  constructor(private authorizeService: AuthorizeService) {
    this.setData();
  }

  ngOnInit() {
    this.authorizeService.authorizationChange.subscribe(_ => {
      this.setData();
    })
  }

  setData() {
    this.isAuthenticated = this.authorizeService.isAuthenticated();
    this.name = this.authorizeService.getUserName();
  }
}
