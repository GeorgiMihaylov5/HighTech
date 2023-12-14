import { Component, OnInit, Output } from '@angular/core';
import { OverviewFacade } from './services/overview-facade.service';
import { Product } from '../models/product.model';

@Component({
    selector: 'app-overview',
    templateUrl: './overview.component.html',
    styleUrls: ['./overview.component.css'],
    providers: [OverviewFacade]
})
export class OverviewComponent implements OnInit {
    public products: Product[];
    public filteredProducts: Product[];
    public isLoading: boolean = false;

    public priceFilterObj: { min: number, max: number } = { min: null, max: null }
    public selectedSorting: number = null;
    public filterManufactorersObj: { manufacturer: string, isChecked: boolean, isVisible: boolean }[] = [];
    public filterModelsObj: { model: string, isChecked: boolean, isVisible: boolean }[] = [];

    constructor(private overviewFacade: OverviewFacade) {

    }

    public ngOnInit(): void {
        this.isLoading = true;

        this.overviewFacade.loadProducts().subscribe((products: Product[]) => {
            this.isLoading = false;
            this.products = products.filter((p: Product) => p.quantity > 0);
            this.filter();
        });
    }


    public filter() {
        this.filteredProducts = this.products;

        this.priceFilter();
        this.filterManufactorers();
        this.filterModels();

        if (this.selectedSorting != null) {
            this.sortFilter(this.selectedSorting);
        }
    }

    public sortFilter(selected: number): void {
        switch (selected) {
            case 1: {
                this.filteredProducts.sort((a, b) => b.discount - a.discount);
                break;
            }
            case 2: {
                this.filteredProducts.sort((a, b) => a.price - b.price);
                break;
            }
            case 3: {
                this.filteredProducts.sort((a, b) => b.price - a.price);
                break;
            }
        }
        this.selectedSorting = selected;
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
            else{
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
            else{
                f.isVisible = false;
                f.isChecked = false;
            }
        })
    }
}
