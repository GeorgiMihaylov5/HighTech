import { Component } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { ManageServiceFacade } from 'src/app/manage/services/manage-facade.service';
import { ICategory } from 'src/app/models/category.model';
import { Field, TypeCode } from 'src/app/models/field.model';
import { CreateOptions } from 'src/app/models/options.model';
import { Product } from 'src/app/models/product.model';

@Component({
  selector: 'app-create',
  templateUrl: './create.component.html',
  styleUrls: ['./create.component.css']
})
export class CreateComponent {
  public selectedOption: CreateOptions = CreateOptions.Product;

  public product: Product = {
    id: null,
    manufacturer: null,
    model: null,
    warranty: 0,
    price: 0,
    discount: 0,
    quantity: 0,
    image: null,
    categoryName: null,
    fields: []
  };

  public field: Field = {
    fieldName: null,
    typeCode: TypeCode.String,
    value: null
  };

  public category: ICategory = {
    categoryId: null,
    fields: []
  };

  public categories: ICategory[] = [];
  public fields: Field[] = [];
  public typeCodes: { key: number, value: string | TypeCode }[] = this.getTypeCodes();

  constructor(private manageService: ManageServiceFacade,
    private toastr: ToastrService) {
    manageService.getData().subscribe((data: [ICategory[], Field[], Product[]]) => {
      this.categories = data[0];
      this.fields = data[1];

      if (data[1].length > 0) {
        this.category.fields.push({...data[1][0]});
      }
      if (data[0].length > 0) {
        this.product.categoryName = data[0][1].categoryId;
        this.changeFields();
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
    if (this.category.fields.length > 1) {
      this.category.fields.splice(index, 1);
    }

  }

  //TODO when is added is duplicated
  public addField() {
    if (this.fields.length > 0) {
      this.category.fields.push({
        fieldName: this.fields[0].fieldName,
        typeCode: this.fields[0].typeCode,
        value: null
      });
    }
  }

  public save() {
    console.log(this.product)
    console.log(this.field)
    console.log(this.category)

    if (this.selectedOption === CreateOptions.Field) {
      this.manageService.createField(this.field).subscribe((field: Field) => {
        this.fields.push(field);
        this.toastr.success('Field was created!');

        this.field = {
          fieldName: '',
          typeCode: TypeCode.String,
          value: ''
        }
      });
    }
    else if (this.selectedOption === CreateOptions.Category) {
      //Fill with right typeCode
      this.category.fields.forEach(f => {
        f.typeCode = this.fields.find(x => x.fieldName === f.fieldName).typeCode;
      });

      this.manageService.createCategory(this.category).subscribe((category: ICategory) => {
        this.categories.push(category);
        this.toastr.success('Category was created!');

        this.category = {
          categoryId: '',
          fields: [{...this.fields[0]}]
        }
      });
    }
    else if (this.selectedOption === CreateOptions.Product) {
      this.manageService.createProduct(this.product).subscribe((product: Product) => {
        this.toastr.success('Product was created!');

        this.product = {
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
      });
    }
    else {
      this.toastr.error("The item wasn't created!");
    }
  }

  private getTypeCodes(): { key: number, value: string | TypeCode }[] {
    return Object.entries(TypeCode)
      .map(([key, value]) => ({ key, value }))
      .filter((v) => isNaN(Number(v.value)))
      .map((v) => ({ key: parseInt(v.key), value: v.value }));
  }
}
