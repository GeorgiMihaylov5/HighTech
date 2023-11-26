import { Component, Input } from '@angular/core';
import { IChangePassword } from '../../models/change-password.model';
import { NgForm } from '@angular/forms';
import { ClientService } from '../../services/client.service';
import { ToastrService } from 'ngx-toastr';
import { Observable, combineLatest } from 'rxjs';
import { IToken } from 'src/api-authorization/models/token.model';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.css']
})
export class ChangePasswordComponent {
  @Input() public token: Observable<IToken>;

  constructor(private clientApi: ClientService,
    private toastr: ToastrService) { }

  public changePassword(form: NgForm) {

    this.token.subscribe((data: IToken) => {
      this.clientApi.changePassword({
        username: data.nameid,
        oldPassword: form.value.oldPassword,
        newPassword: form.value.newPassword,
        confirmNewPassword: form.value.confirmNewPassword
      }).subscribe(_ => {
        this.toastr.success('The password is changed!');
        form.reset();
      });
    });
  }
}
