import { Component } from '@angular/core';
import { EmployeeService } from 'src/app/manage/services/employee.service';
import { IEmployee } from 'src/app/models/employee.model';

@Component({
  selector: 'app-employees',
  templateUrl: './employees.component.html',
  styleUrls: ['./employees.component.css']
})
export class EmployeesComponent {
  public employees: IEmployee[];

  constructor(private employeeApi: EmployeeService) {
    employeeApi.getEmployees().subscribe(emps => {
      this.employees = emps.filter(e => e.username != 'admin');
    })
  }

  public promote(employee: IEmployee): void  {
    this.employeeApi.promote(employee).subscribe(_ => {
      employee.isAdmin = true;
    })
  }

  public demote(employee: IEmployee): void  {
    this.employeeApi.demote(employee).subscribe(_ => {
      employee.isAdmin = false;
    })
  }
}
