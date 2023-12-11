import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { ManageServiceFacade } from 'src/app/manage/services/manage-facade.service';
import { ICategory } from 'src/app/models/category.model';
import { Field } from 'src/app/models/field.model';
import { CreateOptions } from 'src/app/models/options.model';
import { Product } from 'src/app/models/product.model';

@Component({
  selector: 'app-products-table',
  templateUrl: './table.component.html',
  styleUrls: ['./table.component.css']
})
export class TableComponent implements OnInit {
  public selectedOption: CreateOptions = CreateOptions.Product;
  public properties: string[];

  public products: Product[];
  public fields: Field[];
  public categories: ICategory[];

  constructor(
    public manageService: ManageServiceFacade,
    private toastr: ToastrService
  ) {
    this.manageService.getData().subscribe((data) => {
      this.categories = data[0];
      this.fields = data[1];
      this.products = data[2];
    });
  }

  public ngOnInit(): void {

  }

  public getFieldsNames(category: ICategory): string {
    return category.fields.map((c: Field) => c.fieldName).toString()
  }

  public deleteObj(id: string, selectedOption: CreateOptions) {
    this.manageService.delete(id, selectedOption).subscribe((isRemoved: boolean) => {
      if (isRemoved) {
        if (selectedOption === CreateOptions.Field) {
          this.splice(this.fields, 'fieldName', id, 'field');
        }
        else if (selectedOption === CreateOptions.Category) {
          this.splice(this.categories, 'categoryId', id, 'category');
        }
        else if (selectedOption === CreateOptions.Product) {
          this.splice(this.products, 'id', id, 'product');
        }
      }
    });
  }

  private splice(arr: any[], prop: string, id: string, messageObj: string) {
    const indexToRemove = arr.findIndex(obj => obj[prop] === id);

    if (indexToRemove !== -1) {
      arr.splice(indexToRemove, 1);
      this.toastr.success(`The ${messageObj} was successfully removed!`);
    }
  }
}
