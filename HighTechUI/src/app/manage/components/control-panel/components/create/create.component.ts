import { Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Observable, switchMap } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { ManageServiceFacade } from 'src/app/manage/services/manage-facade.service';
import { Category } from 'src/app/models/category.model';
import { Field, TypeCode, TypeCodeOption, UI_TYPE_CODE_OPTIONS, getTypeCodeLabel } from 'src/app/models/field.model';
import { CreateOptions } from 'src/app/manage/models/options.model';
import { Product } from 'src/app/models/product.model';
import { State } from 'src/app/core/state.service';
import { Router } from '@angular/router';
import { NgForm } from '@angular/forms';
import { IEmployee } from 'src/app/manage/models/employee.model';
import { CategoryRemovalPreview } from 'src/app/models/category-removal-preview.model';

@Component({
	selector: 'app-create',
	templateUrl: './create.component.html',
	styleUrls: ['./create.component.css'],
	standalone: false
})
export class CreateComponent implements OnInit, OnDestroy {
	@ViewChild('imageInput') private imageInput?: ElementRef<HTMLInputElement>;

	public selectedOption: CreateOptions = CreateOptions.Product;
	public originalProductPrice: number | null = null;
	public selectedImageFile: File | null = null;
	public imagePreviewUrl: string | null = null;
	private objectUrl: string | null = null;

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
		name: null,
		typeCode: TypeCode.String,
		value: null
	};

	public category: Category = {
		id: null,
		name: null,
		fields: []
	};

	public employee: IEmployee = {
		id: null,
		userId: null,
		username: null,
		email: null,
		firstName: null,
		isAdmin: false,
		lastName: null,
		phoneNumber: null,
		jobTitle: null,
	};

	public categories: Category[] = [];
	public fields: Field[] = [];
	public get typeCodes(): TypeCodeOption[] {
		const editingLegacy = this.isEdit
			&& this.selectedOption === CreateOptions.Field
			&& this.field?.typeCode != null
			&& !UI_TYPE_CODE_OPTIONS.some(o => o.value === this.field.typeCode);

		if (editingLegacy) {
			return [
				...UI_TYPE_CODE_OPTIONS,
				{ value: this.field.typeCode, label: getTypeCodeLabel(this.field.typeCode) }
			];
		}

		return UI_TYPE_CODE_OPTIONS;
	}

	public isCategoryEditDialogOpen = false;
	public categoryEditPreview: CategoryRemovalPreview | null = null;
	public categoryEditPreviewLoading = false;
	public categoryEditSaving = false;

	constructor(private manageService: ManageServiceFacade,
		private toastr: ToastrService,
		private state: State,
		private router: Router) {
		manageService.getCategoryAndFieldData().subscribe(([categories, fields]: [Category[], Field[]]) => {
			this.categories = categories;
			this.fields = fields;

			if (fields.length > 0 && !this.isEdit) {
				this.category.fields.push({ ...fields[0] });
			}

			if (this.isEdit && this.state.option === CreateOptions.Product) {
				this.syncProductFieldsWithSelectedCategory();
			}

			if (categories.length > 0 && !this.isEdit) {
				this.product.categoryName = categories[0].name;
				this.changeFields();
			}
		});
	}

	ngOnDestroy(): void {
		this.state.isEdit = false;
		this.state.editObj = null;
		this.revokeObjectUrl();
	}

	ngOnInit(): void {
		if (this.isEdit) {
			this.selectedOption = this.state.option;

			switch (this.state.option) {
				case CreateOptions.Product: {
					this.product = this.state.editObj as Product;
					this.originalProductPrice = Number(this.product.price);
					this.imagePreviewUrl = this.product.image;
					this.product.fields.forEach((f: Field) => {
						f.value = this.castFieldValue(f);
					})
					this.syncProductFieldsWithSelectedCategory();

					break;
				}
				case CreateOptions.Field: {
					this.field = this.state.editObj as Field;
					break;
				}
				case CreateOptions.Category: {
					this.category = {
						...(this.state.editObj as Category),
						fields: [...(this.state.editObj as Category).fields]
					};
					break;
				}
			}
		}
	}

	public get isEdit(): boolean {
		return this.state.isEdit;
	}

	public get shouldWarnDiscountReset(): boolean {
		return this.isEdit
			&& this.selectedOption === CreateOptions.Product
			&& this.product.discount > 0
			&& this.originalProductPrice != null
			&& Number(this.product.price) !== this.originalProductPrice;
	}

	public getSelectedOptionName(): string {
		switch (this.selectedOption) {
			case CreateOptions.Field:
				return 'Field';
			case CreateOptions.Category:
				return 'Category';
			case CreateOptions.Employee:
				return 'Employee';
			default:
				return 'Product';
		}
	}

	public getSelectedOptionDescription(): string {
		switch (this.selectedOption) {
			case CreateOptions.Field:
				return 'Define reusable product specification fields';
			case CreateOptions.Category:
				return 'Group products by type and shared specifications';
			case CreateOptions.Employee:
				return 'Create a staff account for management workflows';
			default:
				return 'Manage product details, inventory, images, and specifications';
		}
	}

	public changeFields(): void {
		this.syncProductFieldsWithSelectedCategory();
	}

	public onImageSelected(event: Event): void {
		const input = event.target as HTMLInputElement;
		const file = input.files && input.files.length > 0 ? input.files[0] : null;

		this.revokeObjectUrl();
		this.selectedImageFile = file;

		if (file) {
			this.objectUrl = URL.createObjectURL(file);
			this.imagePreviewUrl = this.objectUrl;
		} else {
			// Selection cleared: fall back to the product's saved image (if any).
			this.imagePreviewUrl = this.product.image;
		}
	}

	public triggerImagePicker(): void {
		this.imageInput?.nativeElement.click();
	}

	// Cancels a newly picked file and reverts the preview to the product's saved image (if any).
	public clearImage(): void {
		this.revokeObjectUrl();
		this.selectedImageFile = null;
		this.imagePreviewUrl = this.product.image;

		if (this.imageInput) {
			this.imageInput.nativeElement.value = '';
		}
	}

	private resetImageSelection(): void {
		this.revokeObjectUrl();
		this.selectedImageFile = null;
		this.imagePreviewUrl = null;

		if (this.imageInput) {
			this.imageInput.nativeElement.value = '';
		}
	}

	private revokeObjectUrl(): void {
		if (this.objectUrl) {
			URL.revokeObjectURL(this.objectUrl);
			this.objectUrl = null;
		}
	}

	private syncProductFieldsWithSelectedCategory(): void {
		const selectedCategory = this.categories.find(c => c.name === this.product.categoryName);

		if (selectedCategory && !this.compareArrays(this.product.fields, selectedCategory.fields)) {
			const currentFields = this.product.fields;
			this.product.fields = selectedCategory.fields.map((field: Field): Field => ({
				...field,
				value: (currentFields.find(currentField => currentField.id === field.id)?.value ?? null) as any
			}));
		}
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

	public getFieldMin(field: Field): string | null {
		return this.getFieldType(field) === 'number' ? '0' : null;
	}

	public getFieldStep(field: Field): string | null {
		switch (field.typeCode) {
			case TypeCode.Int16:
			case TypeCode.UInt16:
			case TypeCode.Int32:
			case TypeCode.UInt32:
			case TypeCode.Int64:
			case TypeCode.UInt64:
				return '1';
			case TypeCode.Single:
			case TypeCode.Double:
			case TypeCode.Decimal:
				return 'any';
			default:
				return null;
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

	public save(form?: NgForm) {
		if (form && form.invalid) {
			this.toastr.error('Please fix the highlighted fields before saving.');
			return;
		}

		if (this.selectedOption === CreateOptions.Product && this.product.discount < 0) {
			this.toastr.error('Discount cannot be negative.');
			return;
		}

		if (this.selectedOption === CreateOptions.Product && !this.selectedImageFile && !this.product.image) {
			this.toastr.error('Please upload a product image.');
			return;
		}

		if (this.selectedOption === CreateOptions.Category) {
			this.category.fields.forEach(f => {
				const rightField = this.fields.find((x: Field) => x.name === f.name);
				f.typeCode = rightField.typeCode;
				f.id = rightField.id;
			});

			if (this.hasDuplicateCategoryFields()) {
				this.toastr.error('Category cannot have two same fields.');
				return;
			}
		}

		if (this.isEdit && this.selectedOption === CreateOptions.Category) {
			this.openCategoryEditDialog();
			return;
		}

		this.performSave();
	}

	public openCategoryEditDialog() {
		const removedFieldIds = this.getRemovedCategoryFieldIds();

		if (removedFieldIds.length === 0) {
			this.performSave();
			return;
		}

		this.categoryEditPreview = null;
		this.categoryEditPreviewLoading = true;
		this.isCategoryEditDialogOpen = true;

		this.manageService.previewCategoryFieldRemoval(this.category.id, removedFieldIds).subscribe({
			next: (preview: CategoryRemovalPreview) => {
				this.categoryEditPreviewLoading = false;
				if (preview.totalAffectedProducts === 0) {
					this.isCategoryEditDialogOpen = false;
					this.performSave();
					return;
				}
				this.categoryEditPreview = preview;
			},
			error: () => {
				this.categoryEditPreviewLoading = false;
				this.isCategoryEditDialogOpen = false;
			}
		});
	}

	public closeCategoryEditDialog() {
		this.isCategoryEditDialogOpen = false;
		this.categoryEditPreview = null;
		this.categoryEditPreviewLoading = false;
		this.categoryEditSaving = false;
	}

	public confirmCategoryEdit() {
		if (this.categoryEditSaving) {
			return;
		}

		this.categoryEditSaving = true;
		this.performSave();
	}

	private performSave() {
		if (this.isEdit) {
			if (this.selectedOption === CreateOptions.Product && this.shouldWarnDiscountReset) {
				this.product.discount = 0;
			}

			this.product.fields.forEach(p => {
				p.value = String(p.value);
			})

			this.withProductImageUpload(() =>
				this.manageService.editObj([this.field, this.category, this.product], this.selectedOption)
			).subscribe({
				next: (data: Field | Category | Product) => {
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
				},
				error: () => {
					this.categoryEditSaving = false;
				}
			});
		}
		else {
			this.withProductImageUpload(() =>
				this.manageService.createObj([this.field, this.category, this.product, this.employee], this.selectedOption)
			).subscribe((data: Field | Category | Product | IEmployee) => {
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
						this.resetImageSelection();
						break;
					}
					case CreateOptions.Employee: {
						this.toastr.success('Employee was created!');

						this.employee = {
							id: null,
							userId: null,
							username: null,
							email: null,
							firstName: null,
							isAdmin: false,
							lastName: null,
							phoneNumber: null,
							jobTitle: null,
						};
						break;
					}
				}
			});
		}

		this.state.overviewLoaded = false;
	}

	// For products with a newly picked file, upload it first, set the returned path on the
	// product, then run the save. The save observable is built lazily inside switchMap so the
	// uploaded path is already on product.image when the create/edit request is constructed.
	private withProductImageUpload<T>(buildSave: () => Observable<T>): Observable<T> {
		if (this.selectedOption === CreateOptions.Product && this.selectedImageFile) {
			return this.manageService.uploadProductImage(this.selectedImageFile).pipe(
				switchMap((path: string) => {
					this.product.image = path;
					return buildSave();
				})
			);
		}

		return buildSave();
	}

	private hasDuplicateCategoryFields(): boolean {
		const fieldIds = this.category.fields
			.map(field => field.id)
			.filter(id => id != null);

		return new Set(fieldIds).size !== fieldIds.length;
	}

	private getRemovedCategoryFieldIds(): string[] {
		const originalCategory = this.state.editObj as Category;
		const currentFieldIds = new Set(this.category.fields
			.map(field => field.id)
			.filter(id => id != null));

		return (originalCategory?.fields ?? [])
			.map(field => field.id)
			.filter(id => id != null && !currentFieldIds.has(id));
	}
}
