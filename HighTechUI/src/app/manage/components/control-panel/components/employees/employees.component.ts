import { Component } from '@angular/core';
import { EmployeeService } from 'src/app/manage/services/employee.service';
import { IEmployee } from 'src/app/manage/models/employee.model';
import { AuthorizeService } from 'src/api-authorization/services/authorize-facade.service';

@Component({
	selector: 'app-employees',
	templateUrl: './employees.component.html',
	styleUrls: ['./employees.component.css'],
	standalone: false
})
export class EmployeesComponent {
	public employees: IEmployee[];
	public currentUsername: string | null;

	constructor(private employeeApi: EmployeeService, 
		private authService: AuthorizeService) {
		this.authService.getTokenData().subscribe(token => {
			this.currentUsername = token?.nameid ?? null;
		});

		employeeApi.getEmployees().subscribe(emps => {
			this.employees = emps.filter(e => e.username != 'admin');
		})
	}

	public promote(employee: IEmployee): void {
		this.employeeApi.promote(employee).subscribe(_ => {
			employee.isAdmin = true;
		})
	}

	public demote(employee: IEmployee): void {
		this.employeeApi.demote(employee).subscribe(_ => {
			employee.isAdmin = false;
		})
	}

	public getAdminCount(): number {
		return (this.employees ?? []).filter(employee => employee.isAdmin).length;
	}

	public getStaffCount(): number {
		return (this.employees ?? []).filter(employee => !employee.isAdmin).length;
	}
}
