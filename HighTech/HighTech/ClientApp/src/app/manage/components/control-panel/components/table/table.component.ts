import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { ManageServiceFacade } from 'src/app/manage/services/manage-facade.service';
import { Category } from 'src/app/models/category.model';
import { Field } from 'src/app/models/field.model';
import { CreateOptions } from 'src/app/manage/models/options.model';
import { Product } from 'src/app/models/product.model';
import { State } from 'src/app/core/state.service';
import { Router } from '@angular/router';

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
  public categories: Category[];
  public groupedCategories: Category[] = [];

  constructor(
    public manageService: ManageServiceFacade,
    private state: State,
    private toastr: ToastrService,
    private router: Router
  ) {
    this.manageService.getData().subscribe((data) => {
      this.categories = data[0];
      this.fields = data[1];
      this.products = data[2];

      const addedToGroup: string[] = [];

      this.categories.forEach((item) => {
        if (!addedToGroup.includes(item.name)) {
          this.groupedCategories.push(item);
          addedToGroup.push(item.name);
        }
        else {
          this.groupedCategories.find(f => f.name === item.name).fields.push(...item.fields);
        }
      });
    });
  }


  public ngOnInit(): void {

  }


  public getFieldsNames(category: Category): string {
    return category.fields.map((c: Field) => c.name).toString()
  }

  public deleteObj(id: string, selectedOption: CreateOptions) {
    this.manageService.delete(id, selectedOption).subscribe((isRemoved: boolean) => {
      if (isRemoved) {
        if (selectedOption === CreateOptions.Field) {
          this.splice(this.fields, id, 'field');
        }
        else if (selectedOption === CreateOptions.Category) {
          this.splice(this.categories, id, 'category');
        }
        else if (selectedOption === CreateOptions.Product) {
          this.splice(this.products, id, 'product');
        }
      }
    });
  }

  public editObj(obj: Product | Category | Field, selectedOption: CreateOptions) {
    this.state.editObj = obj;
    this.state.option = selectedOption;
    this.state.isEdit = true;

    this.router.navigateByUrl('/manage/(manage:control-panel/(control-panel:create))');
  }

  private splice(arr: any[], id: string, messageObj: string) {
    const indexToRemove = arr.findIndex(obj => obj['id'] === id);

    if (indexToRemove !== -1) {
      arr.splice(indexToRemove, 1);
      this.toastr.success(`The ${messageObj} was successfully removed!`);
    }
  }
}
