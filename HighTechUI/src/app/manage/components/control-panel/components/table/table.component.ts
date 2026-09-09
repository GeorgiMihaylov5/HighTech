import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { ManageServiceFacade } from 'src/app/manage/services/manage-facade.service';
import { Category } from 'src/app/models/category.model';
import { Field, getTypeCodeLabel } from 'src/app/models/field.model';
import { CreateOptions } from 'src/app/manage/models/options.model';
import { Product } from 'src/app/models/product.model';
import { State } from 'src/app/core/state.service';
import { Router } from '@angular/router';
import { CategoryRemovalPreview } from 'src/app/models/category-removal-preview.model';

@Component({
	selector: 'app-products-table',
	templateUrl: './table.component.html',
	styleUrls: ['./table.component.css'],
	standalone: false
})
export class TableComponent implements OnInit {
	public selectedOption: CreateOptions = CreateOptions.Product;
	public properties: string[];

	public products: Product[];
	public fields: Field[];
	public categories: Category[];
	public productSearch = '';
	public fieldSearch = '';
	public categorySearch = '';
	public isDiscountDialogOpen = false;
	public isDiscountSaving = false;
	public selectedDiscountProduct: Product | null = null;
	public discountPercentage: number | null = null;

	public isCategoryDeleteDialogOpen = false;
	public categoryDeletePreview: CategoryRemovalPreview | null = null;
	public categoryDeletePreviewLoading = false;
	public categoryDeleteSaving = false;
	private categoryToDelete: Category | null = null;

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
		});
	}


	public ngOnInit(): void {

	}


	public get filteredProducts(): Product[] {
		const terms = this.productSearch.trim().toLowerCase().split(/\s+/).filter(Boolean);

		if (!terms.length) {
			return this.products ?? [];
		}

		return (this.products ?? []).filter(product => {
			const haystack = `${product.manufacturer ?? ''} ${product.model ?? ''}`.toLowerCase();
			return terms.every(term => haystack.includes(term));
		});
	}

	public get filteredFields(): Field[] {
		const terms = this.fieldSearch.trim().toLowerCase().split(/\s+/).filter(Boolean);

		if (!terms.length) {
			return this.fields ?? [];
		}

		return (this.fields ?? []).filter(field => {
			const haystack = (field.name ?? '').toLowerCase();
			return terms.every(term => haystack.includes(term));
		});
	}

	public get filteredCategories(): Category[] {
		const terms = this.categorySearch.trim().toLowerCase().split(/\s+/).filter(Boolean);

		if (!terms.length) {
			return this.categories ?? [];
		}

		return (this.categories ?? []).filter(category => {
			const haystack = (category.name ?? '').toLowerCase();
			return terms.every(term => haystack.includes(term));
		});
	}

	public getFieldsNames(category: Category): string {
		return category.fields.map((c: Field) => c.name).toString()
	}

	public getTypeCodeLabel(typeCode: number): string {
		return getTypeCodeLabel(typeCode);
	}

	public deleteObj(id: string, selectedOption: CreateOptions) {
		if (selectedOption === CreateOptions.Category) {
			const category = this.categories.find(c => c.id === id);
			if (category != null) {
				this.openCategoryDeleteDialog(category);
			}
			return;
		}

		this.manageService.delete(id, selectedOption).subscribe((isRemoved: boolean) => {
			if (isRemoved) {
				if (selectedOption === CreateOptions.Field) {
					this.splice(this.fields, id, 'id', 'field');
				}
				else if (selectedOption === CreateOptions.Product) {
					this.splice(this.products, id, 'id', 'product');
				}
			}
		});
	}

	public openCategoryDeleteDialog(category: Category) {
		this.categoryToDelete = category;
		this.categoryDeletePreview = null;
		this.categoryDeletePreviewLoading = true;
		this.isCategoryDeleteDialogOpen = true;

		this.manageService.previewCategoryRemoval(category.id).subscribe({
			next: (preview: CategoryRemovalPreview) => {
				this.categoryDeletePreview = preview;
				this.categoryDeletePreviewLoading = false;
			},
			error: () => {
				this.categoryDeletePreviewLoading = false;
				this.closeCategoryDeleteDialog();
			}
		});
	}

	public closeCategoryDeleteDialog() {
		this.isCategoryDeleteDialogOpen = false;
		this.categoryToDelete = null;
		this.categoryDeletePreview = null;
		this.categoryDeletePreviewLoading = false;
		this.categoryDeleteSaving = false;
	}

	public confirmCategoryDelete() {
		if (this.categoryToDelete == null || this.categoryDeleteSaving) {
			return;
		}

		const id = this.categoryToDelete.id;
		this.categoryDeleteSaving = true;

		this.manageService.delete(id, CreateOptions.Category).subscribe({
			next: (isRemoved: boolean) => {
				if (isRemoved) {
					this.splice(this.categories, id, 'id', 'category');
				}
				this.closeCategoryDeleteDialog();
			},
			error: () => {
				this.categoryDeleteSaving = false;
			}
		});
	}

	public editObj(obj: Product | Category | Field, selectedOption: CreateOptions) {
		this.state.editObj = obj;
		this.state.option = selectedOption;
		this.state.isEdit = true;

		this.router.navigateByUrl('/manage/(manage:control-panel/(control-panel:create))');
	}

	public detailProduct(product: Product) {
		this.state.selectedProduct = product;
		this.router.navigate(["/detail", product.id]);
	}

	public openSetDiscountDialog(product: Product) {
		this.selectedDiscountProduct = product;
		this.discountPercentage = this.getDiscountPercentage(product);
		this.isDiscountDialogOpen = true;
	}

	public closeSetDiscountDialog() {
		this.isDiscountDialogOpen = false;
		this.isDiscountSaving = false;
		this.selectedDiscountProduct = null;
		this.discountPercentage = null;
	}

	public clearDiscount() {
		if (this.selectedDiscountProduct == null || this.isDiscountSaving) {
			return;
		}

		this.isDiscountSaving = true;
		this.manageService.removeDiscount(this.selectedDiscountProduct).subscribe({
			next: (product: Product) => {
				this.applyDiscountResult(product);
				this.closeSetDiscountDialog();
			},
			error: () => {
				this.isDiscountSaving = false;
			}
		});
	}

	public saveDiscount() {
		if (this.selectedDiscountProduct == null || this.isDiscountSaving || this.isDiscountPercentageInvalid) {
			return;
		}

		if (this.discountPercentage === 0) {
			this.clearDiscount();
			return;
		}

		this.isDiscountSaving = true;
		this.manageService.setDiscount(this.selectedDiscountProduct, this.discountPercentage).subscribe({
			next: (product: Product) => {
				this.applyDiscountResult(product);
				this.closeSetDiscountDialog();
			},
			error: () => {
				this.isDiscountSaving = false;
			}
		});
	}

	public get isDiscountPercentageInvalid(): boolean {
		return this.discountPercentage == null
			|| !Number.isInteger(this.discountPercentage)
			|| this.discountPercentage < 0
			|| this.discountPercentage > 100;
	}

	private splice(arr: any[], id: string, prop: string, messageObj: string) {
		const indexToRemove = arr.findIndex(obj => obj[prop] === id);

		if (indexToRemove !== -1) {
			arr.splice(indexToRemove, 1);
			this.toastr.success(`The ${messageObj} was successfully removed!`);
		}
	}

	public getTotalQuantity(): number {
		return (this.products ?? []).reduce((total, product) => total + (Number(product.quantity) || 0), 0);
	}

	public getActiveCount(): number {
		return (this.products ?? []).filter(product => (Number(product.quantity) || 0) > 0).length;
	}

	public getDiscountPercentage(product: Product): number {
		const basePrice = Number(product.price) + Number(product.discount);

		if (product.discount <= 0 || basePrice <= 0) {
			return 0;
		}

		return Math.round((Number(product.discount) / basePrice) * 100);
	}

	private applyDiscountResult(product: Product) {
		if (this.selectedDiscountProduct == null) {
			return;
		}

		this.selectedDiscountProduct.discount = product.discount;
		this.selectedDiscountProduct.price = product.price;
		this.state.updateProduct(product);
	}
}
