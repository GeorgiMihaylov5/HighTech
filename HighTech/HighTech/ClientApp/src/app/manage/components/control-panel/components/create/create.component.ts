import { Component } from '@angular/core';
import { CategoryService } from 'src/app/manage/services/category.service';
import { ManageServiceFacade } from 'src/app/manage/services/manage-facade.service';
import { ICategory } from 'src/app/models/category.model';
import { Field, TypeCode } from 'src/app/models/field.model';
import { Product } from 'src/app/models/product.model';

@Component({
  selector: 'app-create',
  templateUrl: './create.component.html',
  styleUrls: ['./create.component.css']
})
export class CreateComponent {
  public selectedOption: CreateRadioBtn = CreateRadioBtn.Product;

  public product: Product = {
    id: "",
    manufacturer: "",
    model: "",
    warranty: 0,
    price: 0,
    discount: 0,
    quantity: 0,
    image: "",
    categoryName: "",
    fields: []
  };

  public field: Field = {
    fieldName: '',
    typeCode: TypeCode.String,
    value: ''
  };

  public category: ICategory = {
    categoryId: '',
    fields: []
  };

  public categories: ICategory[] = [];
  public fields: Field[] = [];
  public typeCodes: { key: number, value: string | TypeCode }[] = this.getTypeCodes();

  constructor(private manageService: ManageServiceFacade) {
    manageService.getCreateData().subscribe((data: [ICategory[], Field[]]) => {
      this.categories = data[0];
      this.fields = data[1];

      if (data[1].length > 0) {
        this.category.fields.push(data[1][0])
      }
    })
  }

  public changeFields() {
    this.categories.forEach(c => {
      if (c.categoryId === this.product.categoryName) {
        this.product.fields = c.fields;
      }

      return;
    })
  }

  public getFieldType(typeCode: TypeCode): string {
    switch (typeCode) {
      case TypeCode.Int16:
      case TypeCode.UInt16:
      case TypeCode.Int32:
      case TypeCode.UInt32:
      case TypeCode.Int64:
      case TypeCode.UInt64:
      case TypeCode.Single:
      case TypeCode.Double:
      case TypeCode.Decimal:
        return 'number';
      case TypeCode.DateTime:
        return 'datetime-local';
      case TypeCode.Boolean:
        return 'checkbox';
      default:
        return 'text';
    }
  }

  public removeField(index: number) {
    if (this.category.fields.length > 0) {
      this.category.fields.splice(index, 1);
    }

  }
  public addField() {
    if (this.fields.length > 0) {
      this.category.fields.push(this.fields[0]);
    }
  }

  public save() {
    console.log(this.product)
    console.log(this.field)
    console.log(this.category)

    // // Check if an option is selected
    // if (this.selectedOption) {
    //   // Call your API service method based on the selected option
    //   switch (this.selectedOption) {
    //     case 'product':
    //       this.apiService.createProduct(this.productData).subscribe(response => {
    //         console.log('Product created:', response);
    //       });
    //       break;
    //     case 'field':
    //       this.apiService.createField(this.fieldData).subscribe(response => {
    //         console.log('Field created:', response);
    //       });
    //       break;
    //     case 'category':
    //       this.apiService.createCategory(this.categoryData).subscribe(response => {
    //         console.log('Category created:', response);
    //       });
    //       break;
    //     default:
    //       console.error('Invalid option selected');
    //   }
    // } else {
    //   console.error('Please select an option before saving');
    // }
  }



  private getTypeCodes(): { key: number, value: string | TypeCode }[] {
    return Object.entries(TypeCode)
      .map(([key, value]) => ({ key, value }))
      .filter((v) => isNaN(Number(v.value)))
      .map((v) => ({ key: parseInt(v.key), value: v.value }));
  }
}

export enum CreateRadioBtn {
  Product = 0,
  Field = 1,
  Category = 2
}
