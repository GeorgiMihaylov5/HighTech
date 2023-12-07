import { Injectable } from "@angular/core";
import { Observable, combineLatest, map, of, switchMap } from "rxjs";
import { IToken } from "src/api-authorization/models/token.model";
import { AuthorizeService } from "src/api-authorization/services/authorize-facade.service";
import { ClientService } from "./client.service";
import { EmployeeService } from "./employee.service";
import { IEmployee } from "src/app/models/employee.model";
import { IClient } from "src/app/models/client.model";
import { IChangePassword } from "../../models/change-password.model";
import { CategoryService } from "./category.service";
import { FieldService } from "./field.service";
import { ICategory } from "src/app/models/category.model";
import { Field } from "src/app/models/field.model";

@Injectable()
export class ManageServiceFacade {
    public token: Observable<IToken>;

    constructor(private authService: AuthorizeService,
        private clientApi: ClientService,
        private employeeApi: EmployeeService,
        private categoryApi: CategoryService,
        private fieldApi: FieldService
    ) {
        this.token = authService.getTokenData();
    }

    public getProfileData(): Observable<IClient | IEmployee> {
        return this.token.pipe(
            switchMap((tokenData: IToken) => {
                if (tokenData.role.includes('Employee') || tokenData.role.includes('Administrator')) {
                    return this.employeeApi.getEmployee(tokenData.nameid).pipe(
                        map((emp: IEmployee) => {
                            return emp;
                        })
                    );
                }
                else {
                    return this.clientApi.getClient(tokenData.nameid).pipe(
                        map((client): IClient => {
                            return client;
                        })
                    );
                }
            })
        );
    }

    public editProfile(value: IClient | IEmployee): Observable<IClient | IEmployee> {
        if ('jobTitle' in value) {
            return this.employeeApi.editEmployee(value).pipe(
                map((emp: IEmployee) => {
                    return emp;
                })
            )
        }
        else {
            return this.clientApi.editClient(value).pipe(
                map((client: IClient) => {
                    return client;
                })
            )
        }
    }

    public changePassword(changeModel: IChangePassword): Observable<void> {
        return this.token.pipe(
            switchMap((tokenData: IToken) => {
                if (changeModel.username == null) {
                    changeModel.username = tokenData.nameid;
                }

                return this.clientApi.changePassword(changeModel).pipe(
                    switchMap(_ => {
                        return of(null);
                    })
                );
            })
        );
    }

    public getCreateData(): Observable<[ICategory[], Field[]]> {
        return combineLatest([
            this.categoryApi.getCategories(),
            this.fieldApi.getFields()
        ]);
    }
}