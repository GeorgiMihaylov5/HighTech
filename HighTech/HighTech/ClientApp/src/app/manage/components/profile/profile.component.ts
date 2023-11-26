import { Component, Input, OnInit } from '@angular/core';
import { IClient } from 'src/app/models/client.model';
import { ClientService } from '../../services/client.service';
import { Observable, tap } from 'rxjs';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { IToken } from 'src/api-authorization/models/token.model';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {
  @Input() token: Observable<IToken>;

   public client: IClient = {
    id: '',
    firstName: '',
    lastName: '',
    username: '',
    address: '',
    phoneNumber: '',
    email: ''
  };

  public originalValue: IClient;

  constructor(private clientApi: ClientService,
    private toastr: ToastrService) {

      
  }
  ngOnInit(): void {

    this.token.subscribe(data => {
      this.clientApi.getClient(data.nameid).subscribe(client => {
        this.client = client;
        this.originalValue = { ...client };
      })
    })
  }

  editProfile(form: NgForm) {
    this.clientApi.editUser(form.value).subscribe((client: IClient) => {
      this.client = client;
      this.originalValue = { ...client };
      this.toastr.success('You successfully update your profile!')
    })
  }

}
