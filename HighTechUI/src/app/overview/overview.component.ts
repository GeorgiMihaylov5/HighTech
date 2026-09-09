import { AfterViewInit, Component, ElementRef, HostListener, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { OverviewFacade } from './services/overview-facade.service';
import { Product } from '../models/product.model';
import { combineLatest, Subject, takeUntil } from 'rxjs';

@Component({
	selector: 'app-overview',
	templateUrl: './overview.component.html',
	styleUrls: ['./overview.component.css'],
	standalone: false
})
export class OverviewComponent implements OnInit, AfterViewInit, OnDestroy {
	@ViewChild('filterPanel') private filterPanel: ElementRef<HTMLElement>;

	public products: Product[];
	public filteredProducts: Product[];
	public pagedProducts: Product[] = [];
	public currentPage: number = 1;
	public readonly pageSize: number = 24;
	public totalPages: number = 1;
	public isLoading: boolean = false;
	public isFilterPanelOpen: boolean = false;
	public filterPanelMaxHeight: number = null;
	private destroy$ = new Subject<void>();

	public priceFilterObj: { min: number, max: number } = { min: null, max: null }
	public selectedSorting: number = null;
	public selectedCategoryName: string = null;
	public showPromotionsOnly: boolean = false;
	public filterManufactorersObj: { manufacturer: string, isChecked: boolean, isVisible: boolean }[] = [];
	public filterModelsObj: { model: string, isChecked: boolean, isVisible: boolean }[] = [];
	public categoryOptions: string[] = [];
	public categoryFieldFilters: {
		fieldId: string,
		fieldName: string,
		values: { value: string, isChecked: boolean }[]
	}[] = [];

	constructor(
		private overviewFacade: OverviewFacade,
		private router: Router,
		private route: ActivatedRoute
	) {

	}

	public ngOnInit(): void {
		this.isLoading = true;
		const navigationState = this.router.getCurrentNavigation()?.extras.state as { showPromotionsOnly?: boolean };
		this.showPromotionsOnly = navigationState?.showPromotionsOnly ?? history.state?.showPromotionsOnly ?? false;

		combineLatest([
			this.overviewFacade.loadProducts(),
			this.route.queryParamMap
		]).pipe(takeUntil(this.destroy$)).subscribe(([products, queryParamMap]) => {
			this.isLoading = false;
			this.products = products.filter((p: Product) => p.quantity > 0);
			this.categoryOptions = this.getCategoryOptions(this.products);

			const categoryFromQuery = queryParamMap.get('category');

			if (categoryFromQuery && this.categoryOptions.includes(categoryFromQuery)) {
				this.onCategoryChanged(categoryFromQuery, false);
			}
			else {
				this.onCategoryChanged(null, false);
			}

			this.filter();
			this.scheduleFilterPanelHeightUpdate();
		});
	}

	public ngAfterViewInit(): void {
		this.scheduleFilterPanelHeightUpdate();
	}

	public ngOnDestroy(): void {
		this.destroy$.next();
		this.destroy$.complete();
	}

	@HostListener('window:resize')
	public onWindowResize(): void {
		this.scheduleFilterPanelHeightUpdate();
	}

	@HostListener('window:scroll')
	public onWindowScroll(): void {
		this.updateFilterPanelMaxHeight();
	}

	public filter() {
		this.currentPage = 1;
		this.filteredProducts = [...this.products];

		this.promotionsFilter();
		this.categoryFilter();
		this.categoryFieldsFilter();
		this.priceFilter();
		this.filterManufactorers();
		this.filterModels();

		if (this.selectedSorting != null) {
			this.sortFilter(this.selectedSorting);
		}

		this.updatePagedProducts();
	}

	public goToPage(page: number): void {
		if (page < 1 || page > this.totalPages || page === this.currentPage) {
			return;
		}
		this.currentPage = page;
		this.updatePagedProducts();
		window.scrollTo({ top: 0, behavior: 'smooth' });
	}

	public get pageNumbers(): number[] {
		const maxVisible = 5;

		if (this.totalPages <= maxVisible) {
			return Array.from({ length: this.totalPages }, (_, i) => i + 1);
		}

		const half = Math.floor(maxVisible / 2);
		let start = this.currentPage - half;
		let end = this.currentPage + half;

		if (start < 1) {
			start = 1;
			end = maxVisible;
		} else if (end > this.totalPages) {
			end = this.totalPages;
			start = this.totalPages - maxVisible + 1;
		}

		const pages: number[] = [];
		for (let i = start; i <= end; i++) {
			pages.push(i);
		}
		return pages;
	}

	private updatePagedProducts(): void {
		this.totalPages = Math.max(1, Math.ceil(this.filteredProducts.length / this.pageSize));
		if (this.currentPage > this.totalPages) {
			this.currentPage = this.totalPages;
		}
		const start = (this.currentPage - 1) * this.pageSize;
		this.pagedProducts = this.filteredProducts.slice(start, start + this.pageSize);
	}

	public onCategoryChanged(categoryName: string, shouldFilter: boolean = true): void {
		const selectedCategoryName = categoryName || null;
		const hasCategoryChanged = this.selectedCategoryName !== selectedCategoryName;

		this.selectedCategoryName = selectedCategoryName;
		this.categoryFieldFilters = this.selectedCategoryName
			? this.getCategoryFieldFilters(this.selectedCategoryName)
			: [];

		if (hasCategoryChanged) {
			this.resetManufacturerAndModelFilters();
		}

		if (shouldFilter && this.products) {
			this.filter();
		}
	}

	public togglePromotionsFilter(): void {
		this.showPromotionsOnly = !this.showPromotionsOnly;
		this.filter();
	}

	public toggleFilterPanel(): void {
		this.isFilterPanelOpen = !this.isFilterPanelOpen;
	}

	public closeFilterPanel(): void {
		this.isFilterPanelOpen = false;
	}

	public applyFilters(): void {
		this.filter();
		this.isFilterPanelOpen = false;
	}

	public sortFilter(selected: number): void {
		switch (selected) {
			case 1: {
				this.filteredProducts.sort((a, b) => a.price - b.price);
				break;
			}
			case 2: {
				this.filteredProducts.sort((a, b) => b.price - a.price);
				break;
			}
		}
		this.selectedSorting = selected;
		this.currentPage = 1;
		this.updatePagedProducts();
	}

	public promotionsFilter(): void {
		if (this.showPromotionsOnly) {
			this.filteredProducts = this.filteredProducts.filter(p => p.discount > 0);
		}
	}

	public categoryFilter(): void {
		if (this.selectedCategoryName != null) {
			this.filteredProducts = this.filteredProducts.filter(p => p.categoryName === this.selectedCategoryName);
		}
	}

	public categoryFieldsFilter(): void {
		if (this.categoryFieldFilters.length === 0) {
			return;
		}

		const checkedFieldFilters = this.categoryFieldFilters
			.map(fieldFilter => ({
				fieldId: fieldFilter.fieldId,
				values: fieldFilter.values
					.filter(valueFilter => valueFilter.isChecked)
					.map(valueFilter => valueFilter.value)
			}))
			.filter(fieldFilter => fieldFilter.values.length > 0);

		if (checkedFieldFilters.length === 0) {
			return;
		}

		this.filteredProducts = this.filteredProducts.filter((product: Product) => {
			return checkedFieldFilters.every(fieldFilter => {
				const productField = product.fields?.find(field => field.id === fieldFilter.fieldId);

				return productField != null
					&& fieldFilter.values.includes(this.normalizeFieldValue(productField.value));
			});
		});
	}

	public priceFilter() {
		if (this.priceFilterObj.min != null && this.priceFilterObj.max != null) {
			this.filteredProducts = this.filteredProducts.filter(p => p.price >= this.priceFilterObj.min && p.price <= this.priceFilterObj.max);
		}
		else if (this.priceFilterObj.min != null) {
			this.filteredProducts = this.filteredProducts.filter(p => p.price >= this.priceFilterObj.min);
		}
		else if (this.priceFilterObj.max != null) {
			this.filteredProducts = this.filteredProducts.filter(p => p.price <= this.priceFilterObj.max);
		}
	}

	public filterManufactorers(): void {
		if (this.filteredProducts && this.filterManufactorersObj.length === 0) {
			this.filterManufactorersObj = [...new Set(this.filteredProducts.map(p => p.manufacturer))]
				.map((manufacturer: string) => ({ manufacturer, isChecked: false, isVisible: true }));
		}
		else {
			this.refreshManufactorers()

			const checkedFilters = this.filterManufactorersObj
				.filter(f => f.isChecked == true && f.isVisible == true);

			if (this.filteredProducts && checkedFilters.length === 0) {
				this.filteredProducts = this.filteredProducts;
			}
			else if (this.filteredProducts) {
				this.filteredProducts = this.filteredProducts
					.filter((p: Product) =>
						checkedFilters
							.map(f => f.manufacturer)
							.includes(p.manufacturer));
			}
		}
	}

	public filterModels(): void {
		if (this.filteredProducts && this.filterModelsObj.length === 0) {
			this.filterModelsObj = [...new Set(this.filteredProducts.map(p => p.model))]
				.map((model: string) => ({ model, isChecked: false, isVisible: true }));
		}
		else {
			this.refreshModels();

			const checkedFilters = this.filterModelsObj
				.filter(f => f.isChecked == true && f.isVisible == true);

			if (this.filteredProducts && checkedFilters.length === 0) {
				this.filteredProducts = this.filteredProducts;
			}
			else if (this.filteredProducts) {
				this.filteredProducts = this.filteredProducts
					.filter((p: Product) =>
						checkedFilters
							.map(f => f.model)
							.includes(p.model));
			}
		}

		this.refreshManufactorers();
	}

	private refreshManufactorers(): void {
		this.filterManufactorersObj.forEach(f => {
			if (this.filteredProducts.map(x => x.manufacturer).includes(f.manufacturer)) {
				f.isVisible = true;
			}
			else {
				f.isVisible = false;
				f.isChecked = false;
			}
		});
	}

	private refreshModels(): void {
		this.filterModelsObj.forEach(f => {
			if (this.filteredProducts.map(x => x.model).includes(f.model)) {
				f.isVisible = true;
			}
			else {
				f.isVisible = false;
				f.isChecked = false;
			}
		})
	}

	private resetManufacturerAndModelFilters(): void {
		this.filterManufactorersObj = [];
		this.filterModelsObj = [];
	}

	private getCategoryOptions(products: Product[]): string[] {
		return [...new Set(products
			.map(product => product.categoryName)
			.filter(categoryName => categoryName != null && categoryName.trim() !== ''))]
			.sort((a, b) => a.localeCompare(b));
	}

	private getCategoryFieldFilters(categoryName: string) {
		const productsInCategory = this.products.filter(product => product.categoryName === categoryName);
		const fieldMap = new Map<string, { fieldId: string, fieldName: string, values: Set<string> }>();

		productsInCategory.forEach(product => {
			product.fields?.forEach(field => {
				const value = this.normalizeFieldValue(field.value);

				if (value === '') {
					return;
				}

				if (!fieldMap.has(field.id)) {
					fieldMap.set(field.id, {
						fieldId: field.id,
						fieldName: field.name,
						values: new Set<string>()
					});
				}

				fieldMap.get(field.id).values.add(value);
			});
		});

		return Array.from(fieldMap.values())
			.sort((a, b) => a.fieldName.localeCompare(b.fieldName))
			.map(fieldFilter => ({
				fieldId: fieldFilter.fieldId,
				fieldName: fieldFilter.fieldName,
				values: Array.from(fieldFilter.values)
					.sort((a, b) => a.localeCompare(b, undefined, { numeric: true }))
					.map(value => ({ value, isChecked: false }))
			}));
	}

	private normalizeFieldValue(value: any): string {
		return value == null ? '' : String(value).trim();
	}

	private scheduleFilterPanelHeightUpdate(): void {
		if (typeof window === 'undefined') {
			return;
		}

		window.requestAnimationFrame(() => this.updateFilterPanelMaxHeight());
	}

	private updateFilterPanelMaxHeight(): void {
		if (typeof window === 'undefined' || this.filterPanel == null) {
			return;
		}

		if (window.innerWidth <= 991) {
			this.filterPanelMaxHeight = null;
			return;
		}

		const viewportBottomGap = 16;
		const panelTop = this.filterPanel.nativeElement.getBoundingClientRect().top;
		this.filterPanelMaxHeight = Math.max(280, Math.floor(window.innerHeight - panelTop - viewportBottomGap));
	}

}
