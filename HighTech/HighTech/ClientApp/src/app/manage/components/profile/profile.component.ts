import { Component, Input, OnInit } from '@angular/core';
import { Client } from 'src/app/manage/models/client.model';
import { Observable } from 'rxjs';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { IToken } from 'src/api-authorization/models/token.model';
import { IEmployee } from '../../models/employee.model';
import { ManageServiceFacade } from '../../services/manage-facade.service';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {
  @Input() token: Observable<IToken>;

  public value: Client | IEmployee = {
    id: '',
    firstName: '',
    lastName: '',
    username: '',
    address: '',
    phoneNumber: '',
    email: '',
    jobTitle: ''
  };

  public isEmp: boolean = false;
  public originalValue: Client | IEmployee;

  constructor(private manageService: ManageServiceFacade,
    private toastr: ToastrService) { }
  ngOnInit(): void {
    this.manageService.getProfileData().subscribe(data => {
      this.value = data;
      this.originalValue = { ...data };
    });
  }

  isClient(value: Client | IEmployee): value is Client {
    return typeof value === 'object' && 'address' in value;
  }

  isEmployee(value: Client | IEmployee): value is IEmployee {
    return typeof value === 'object' && 'jobTitle' in value;
  }

  editProfile(form: NgForm) {
   this.manageService.editProfile(form.value).subscribe(data => {
    this.value = data;
    this.originalValue = { ...data };
    this.toastr.success('You successfully update your profile!');
   })
  }

}
