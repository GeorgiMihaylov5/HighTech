import { Component } from '@angular/core';
import { CategoryService } from 'src/app/manage/services/category.service';
import { ICategory } from 'src/app/models/category.model';
import { Field, TypeCode } from 'src/app/models/field.model';
import { Product } from 'src/app/models/product.model';

@Component({
  selector: 'app-create',
  templateUrl: './create.component.html',
  styleUrls: ['./create.component.css']
})
export class CreateComponent {
  selectedOption: CreateRadioBtn = CreateRadioBtn.Product;
  product: Product = {
    id: "",
    manufacturer: "",
    model: "",
    warranty: 0,
    price: 0,
    discount: 0,
    quantity: 0,
    image: "",
    categoryName: "",
    fields: []  // Assuming Fields is an array of Field interface
  }; // Initialize as an empty object
  fieldData: Partial<Field> = {}; // Initialize as an empty object
  categoryData: Partial<ICategory> = {}; // Initialize as an empty object

  public categories: ICategory[] = [];

  constructor(private categoryApi: CategoryService) {
    categoryApi.getCategories().subscribe(x => {
      this.categories = x;
    })
  }

  changeFields() {
    this.categories.forEach(c => {
      if(c.categoryId === this.product.categoryName) {
        this.product.fields = c.fields;
      }
      
      return;
    })
  }

  getFieldType(typeCode: TypeCode): string {
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



  //typeCodes: number[] = Object.keys(TypeCode).filter((k) => !isNaN(Number(TypeCode[k]))).map(Number);

  //constructor(private apiService: ApiService) {}

  save() {
console.log(this.product)

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
}

export enum CreateRadioBtn {
  Product = 0,
  Field = 1,
  Category = 2
}
