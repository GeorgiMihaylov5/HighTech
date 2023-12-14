import { Component, OnInit, Output } from '@angular/core';
import { OverviewFacade } from './services/overview-facade.service';
import { Product } from '../models/product.model';
import { identifierName } from '@angular/compiler';

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

    public priceFilterObj: { min: number, max: number } = { min: null, max: null}
    public selectedSorting: number = null;

    constructor(private overviewFacade: OverviewFacade) {

    }

    public get Manufactorers(): string[] {
        if (!this.products) {
            return [];
        }

        return [...new Set(this.products.map(p => p.manufacturer))]
    }

    public ngOnInit(): void {
        this.isLoading = true;

        this.overviewFacade.loadProducts().subscribe((products: Product[]) => {
            this.isLoading = false;
            this.products = products.filter((p: Product) => p.quantity > 0);
            this.filteredProducts = products.filter((p: Product) => p.quantity > 0);

            this.sortFilter(1);
        });
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
        if(this.priceFilterObj.min != null && this.priceFilterObj.max != null) {
            this.filteredProducts = this.products.filter(p => p.price >= this.priceFilterObj.min && p.price <= this.priceFilterObj.max);
        }
        else if(this.priceFilterObj.min != null) {
            this.filteredProducts = this.products.filter(p => p.price >= this.priceFilterObj.min);
        }
        else if(this.priceFilterObj.max != null) {
            this.filteredProducts = this.products.filter(p => p.price <= this.priceFilterObj.max);
        }
        else {
            this.filteredProducts = this.products;
        }

        if(this.selectedSorting != null) {
            this.sortFilter(this.selectedSorting);
        }
    }
}
