import { Component, OnDestroy, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { ManageServiceFacade } from 'src/app/manage/services/manage-facade.service';
import { Category } from 'src/app/models/category.model';
import { Field, TypeCode } from 'src/app/models/field.model';
import { CreateOptions } from 'src/app/manage/models/options.model';
import { Product } from 'src/app/models/product.model';
import { State } from 'src/app/core/state.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-create',
  templateUrl: './create.component.html',
  styleUrls: ['./create.component.css']
})
export class CreateComponent implements OnInit, OnDestroy {
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
    id: null,
    name : null,
    typeCode: TypeCode.String,
    value: null
  };

  public category: Category = {
    id: null,
    name: null,
    fields: []
  };

  public categories: Category[] = [];
  public fields: Field[] = [];
  public typeCodes: { key: number, value: string | TypeCode }[] = this.getTypeCodes();
  public groupedCategories: Category[] = [];

  constructor(private manageService: ManageServiceFacade,
    private toastr: ToastrService,
    private state: State,
    private router: Router) {
    manageService.getData().subscribe((data: [Category[], Field[], Product[]]) => {
      this.categories = data[0];
      this.fields = data[1];

      if (data[1].length > 0 && !this.isEdit) {
        this.category.fields.push({ ...data[1][0] });
      }
      if (data[0].length > 0 && !this.isEdit) {
        this.product.categoryName = data[0][1].name;
      }

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

      this.changeFields();
    })
  }

  ngOnDestroy(): void {
    this.state.isEdit = false;
    this.state.editObj = null;
  }

  ngOnInit(): void {
    if (this.isEdit) {
      this.selectedOption = this.state.option;

      switch (this.state.option) {
        case CreateOptions.Product: {
          this.product = this.state.editObj as Product;
          this.product.fields.forEach((f: Field) => {
            f.value = this.castFieldValue(f);
          })

          break;
        }
        case CreateOptions.Field: {
          this.field = this.state.editObj as Field;
          break;
        }
        case CreateOptions.Category: {
          this.category = this.state.editObj as Category;
          break;
        }
      }
    }
  }

  public get isEdit(): boolean {
    return this.state.isEdit;
  }

  public changeFields(): void {
    this.groupedCategories.forEach(c => {
      if (c.name === this.product.categoryName && !this.compareArrays(this.product.fields, c.fields)) {
        this.product.fields = c.fields;
      }

      return;
    })
  }

  public getFieldType(field: Field): string {
    switch (field.typeCode) {
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

  private compareArrays(array1: Field[], array2: Field[]): boolean {
    if (array1.length !== array2.length) {
      return false; 
    }
  
    return array1.every((field1) => {
      return array2.some((field2) => {
        return (
          field1.id === field2.id
        );
      });
    });
  }


  private castFieldValue(field: Field) {
    switch (field.typeCode) {
      case TypeCode.Int16:
      case TypeCode.UInt16:
      case TypeCode.Int32:
      case TypeCode.UInt32:
      case TypeCode.Int64:
      case TypeCode.UInt64:
      case TypeCode.Single:
      case TypeCode.Double:
      case TypeCode.Decimal:
        return Number(field.value);
      case TypeCode.DateTime:
        return field.value as Date;
      case TypeCode.Boolean:
        return field.value as boolean;
      default: 
      return field.value as string;
    }
  }

  public removeField(index: number) {
    if (this.category.fields.length > 1) {
      this.category.fields.splice(index, 1);
    }

  }

  public addField() {
    if (this.fields.length > 0) {
      this.category.fields.push({
        id: this.fields[0].id,
        name: this.fields[0].name,
        typeCode: this.fields[0].typeCode,
        value: null
      });
    }
  }

  public save() {
    console.log(this.product)
    console.log(this.field)
    console.log(this.category)

    //Filling with right typeCode
    if (this.selectedOption === CreateOptions.Category) {
      this.category.fields.forEach(f => {
        const rightField = this.fields.find((x: Field) => x.name === f.name);
        f.typeCode = rightField.typeCode;
        f.id = rightField.id;
      });
    }

    if(this.isEdit) {
      this.product.fields.forEach(p => {
        p.value = String(p.value);
      })

      this.manageService.editObj([this.field, this.category, this.product], this.selectedOption).subscribe((data: Field | Category | Product) => {
        switch (this.selectedOption) {
          case CreateOptions.Field: {
            this.toastr.success('Field was updated!');
            break;
          }
          case CreateOptions.Category: {
            this.toastr.success('Category was updated!');
            break;
          }
          case CreateOptions.Product: {
            this.toastr.success('Product was updated!');
            break;
          }
        }

        this.router.navigateByUrl('/manage/(manage:control-panel/(control-panel:table))');
      });
    }
    else {
      this.manageService.createObj([this.field, this.category, this.product], this.selectedOption).subscribe((data: Field | Category | Product) => {
        switch (this.selectedOption) {
          case CreateOptions.Field: {
            this.toastr.success('Field was created!');
  
            this.fields.push(data as Field);
  
            this.field = {
              id: null,
              name: null,
              typeCode: TypeCode.String,
              value: null
            }
            break;
          }
          case CreateOptions.Category: {
            this.toastr.success('Category was created!');
  
            this.categories.push(data as Category);
  
            this.category = {
              id: null,
              name: null,
              fields: [{ ...this.fields[0] }]
            }
            break;
          }
          case CreateOptions.Product: {
            this.toastr.success('Product was created!');

            this.product = {
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
            break;
          }
        }
      });
    }

    this.state.overviewLoaded = false;
  }

  private getTypeCodes(): { key: number, value: string | TypeCode }[] {
    return Object.entries(TypeCode)
      .map(([key, value]) => ({ key, value }))
      .filter((v) => isNaN(Number(v.value)))
      .map((v) => ({ key: parseInt(v.key), value: v.value }));
  }
}
