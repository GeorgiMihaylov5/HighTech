import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { ManageServiceFacade } from 'src/app/manage/services/manage-facade.service';
import { ICategory } from 'src/app/models/category.model';
import { Field } from 'src/app/models/field.model';
import { CreateOptions } from 'src/app/models/options.model';
import { Product } from 'src/app/models/product.model';

@Component({
  selector: 'app-products-table',
  templateUrl: './products-table.component.html',
  styleUrls: ['./products-table.component.css']
})
export class ProductsTableComponent implements OnInit {
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

  ngOnInit(): void {
    
  }

  getFieldsNames(category: ICategory): string {
    return category.fields.map((c: Field) => c.fieldName).toString()
  }
}
