import { Injectable } from "@angular/core";
import { Observable, combineLatest, map, of, switchMap } from "rxjs";
import { IToken } from "src/api-authorization/models/token.model";
import { AuthorizeService } from "src/api-authorization/services/authorize-facade.service";
import { ClientService } from "./client.service";
import { EmployeeService } from "./employee.service";
import { IEmployee } from "src/app/manage/models/employee.model";
import { Client } from "src/app/manage/models/client.model";
import { IChangePassword } from "../models/change-password.model";
import { CategoryService } from "./category.service";
import { FieldService } from "./field.service";
import { Category } from "src/app/models/category.model";
import { Field } from "src/app/models/field.model";
import { Product } from "src/app/models/product.model";
import { ProductService } from "src/app/services/product.service";
import { CreateOptions } from "src/app/manage/models/options.model";

@Injectable()
export class ManageServiceFacade {
    public token: Observable<IToken>;

    constructor(private authService: AuthorizeService,
        private clientApi: ClientService,
        private employeeApi: EmployeeService,
        private categoryApi: CategoryService,
        private fieldApi: FieldService,
        private productApi: ProductService
    ) {
        this.token = authService.getTokenData();
    }

    public getProfileData(): Observable<Client | IEmployee> {
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
                        map((client): Client => {
                            return client;
                        })
                    );
                }
            })
        );
    }

    public editProfile(value: Client | IEmployee): Observable<Client | IEmployee> {
        if ('jobTitle' in value) {
            return this.employeeApi.editEmployee(value).pipe(
                map((emp: IEmployee) => {
                    return emp;
                })
            )
        }
        else {
            return this.clientApi.editClient(value).pipe(
                map((client: Client) => {
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

    public getData(): Observable<[Category[], Field[], Product[]]> {
        return combineLatest([
            this.categoryApi.getCategories(),
            this.fieldApi.getFields(),
            this.productApi.getProducts()
        ]);
    }

    public delete(id: string, selectedOption: CreateOptions): Observable<boolean> {
        if(selectedOption === CreateOptions.Field) {
            return this.fieldApi.deleteField(id);
        }
        else if(selectedOption === CreateOptions.Category) {
            return this.categoryApi.deleteCategory(id);
        }
        else if(selectedOption === CreateOptions.Product) {
            return this.productApi.deleteProduct(id);
        }

        return of(false);
    }

    public createObj(arr: [Field, Category, Product ], selectedOption: CreateOptions): Observable<Field | Category | Product> {
        if(selectedOption === CreateOptions.Field) {
            return this.fieldApi.createField(arr[0]);
        }
        else if(selectedOption === CreateOptions.Category) {
            return this.categoryApi.createCategory(arr[1]);
        }
        else if(selectedOption === CreateOptions.Product) {
            return this.productApi.createProduct(arr[2]);
        }

        return of(null);
    }

    public editObj(arr: [Field, Category, Product ], selectedOption: CreateOptions): Observable<Field | Category | Product> {
        if(selectedOption === CreateOptions.Field) {
            return this.fieldApi.editField(arr[0]);
        }
        else if(selectedOption === CreateOptions.Category) {
            return this.categoryApi.editCategory(arr[1]);
        }
        else if(selectedOption === CreateOptions.Product) {
            return this.productApi.editProduct(arr[2]);
        }

        return of(null);
    }

    public increaseDiscount(product: Product): Observable<Product> {
        return this.productApi.increaseDiscount(product.id, 5);
    }

    public removeDiscount(product: Product): Observable<Product> {
        return this.productApi.removeDiscount(product.id);
    }
}