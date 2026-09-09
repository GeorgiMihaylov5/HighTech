import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Field } from 'src/app/models/field.model';
import { Product } from 'src/app/models/product.model';
import { ComparisonLoadResult } from '../../services/overview-facade.service';
import { OverviewFacade } from '../../services/overview-facade.service';

interface ComparisonRow {
    label: string;
    values: string[];
}

@Component({
    selector: 'app-compare',
    templateUrl: './compare.component.html',
    styleUrls: ['./compare.component.css'],
    standalone: false
})
export class CompareComponent implements OnInit {
    public products: Product[] = [];
    public isLoading: boolean = true;
    public removedCount: number = 0;

    constructor(private overviewFacade: OverviewFacade,
        private toastr: ToastrService) {
    }

    public ngOnInit(): void {
        this.loadProducts();
    }

    public removeProduct(productId: string): void {
        this.overviewFacade.removeCompare(productId);
        this.products = this.products.filter((product: Product) => product.id !== productId);
    }

    public clearComparison(): void {
        this.overviewFacade.clearCompare();
        this.products = [];
    }

    public get hasMixedCategories(): boolean {
        const categories = this.products
            .map((product: Product) => product.categoryName)
            .filter((categoryName: string) => categoryName != null && categoryName.trim() !== '');

        return new Set(categories).size > 1;
    }

    public get comparisonRows(): ComparisonRow[] {
        const rows: ComparisonRow[] = [
            {
                label: 'Manufacturer',
                values: this.products.map((product: Product) => this.formatValue(product.manufacturer))
            },
            {
                label: 'Model',
                values: this.products.map((product: Product) => this.formatValue(product.model))
            },
            {
                label: 'Price',
                values: this.products.map((product: Product) => this.formatCurrency(product.price))
            }
        ];

        if (this.hasAnyDiscount) {
            rows.push(
                {
                    label: 'Original price',
                    values: this.products.map((product: Product) =>
                        product.discount > 0 ? this.formatCurrency(product.price + product.discount) : '-')
                },
                {
                    label: 'Discount',
                    values: this.products.map((product: Product) =>
                        product.discount > 0 ? this.formatCurrency(product.discount) : '-')
                }
            );
        }

        rows.push(
            {
                label: 'Warranty',
                values: this.products.map((product: Product) => this.formatWarranty(product.warranty))
            },
            {
                label: 'Category',
                values: this.products.map((product: Product) => this.formatValue(product.categoryName))
            },
            {
                label: 'Stock',
                values: this.products.map((product: Product) => this.formatValue(product.quantity))
            },
            {
                label: 'Average rating',
                values: this.products.map((product: Product) => this.formatNumberValue(product.averageRating))
            },
            {
                label: 'Reviews',
                values: this.products.map((product: Product) => this.formatValue(product.reviewCount ?? 0))
            }
        );

        return rows.concat(this.fieldRows);
    }

    private get hasAnyDiscount(): boolean {
        return this.products.some((product: Product) => product.discount > 0);
    }

    private loadProducts(): void {
        this.isLoading = true;
        this.removedCount = 0;

        this.overviewFacade.loadFreshComparisonProducts().subscribe((result: ComparisonLoadResult) => {
            this.products = result.products;
            this.removedCount = result.removedIds.length;
            this.isLoading = false;

            if (this.removedCount > 0) {
                this.toastr.info('Some comparison products are no longer available.');
            }
        });
    }

    private get fieldRows(): ComparisonRow[] {
        const fieldNames = this.products
            .flatMap((product: Product) => product.fields ?? [])
            .filter((field: Field) => this.hasFieldValue(field))
            .map((field: Field) => field.name)
            .filter((name: string, index: number, names: string[]) => names.indexOf(name) === index)
            .sort((a: string, b: string) => a.localeCompare(b));

        return fieldNames.map((fieldName: string) => ({
            label: fieldName,
            values: this.products.map((product: Product) => {
                const field = product.fields?.find((item: Field) => item.name === fieldName);
                return this.hasFieldValue(field) ? this.formatValue(field.value) : '-';
            })
        }));
    }

    private hasFieldValue(field: Field | null | undefined): boolean {
        return field?.value !== null
            && field?.value !== undefined
            && (typeof field.value !== 'string' || !this.isMissingString(field.value));
    }

    public getProductTitle(product: Product): string {
        const title = [product.manufacturer, product.model]
            .filter((value: string) => value != null && value.trim() !== '')
            .join(' ');

        return title === '' ? '-' : title;
    }

    private formatValue(value: unknown): string {
        if (value === null || value === undefined) {
            return '-';
        }

        if (typeof value === 'string' && this.isMissingString(value)) {
            return '-';
        }

        return String(value);
    }

    private isMissingString(value: string): boolean {
        const normalizedValue = value.trim().toLowerCase();

        return normalizedValue === '' || normalizedValue === 'null' || normalizedValue === 'undefined';
    }

    private formatCurrency(value: number | null | undefined): string {
        if (value === null || value === undefined) {
            return '-';
        }

        return `${this.formatNumber(value)} EUR`;
    }

    private formatWarranty(value: number | null | undefined): string {
        if (value === null || value === undefined) {
            return '-';
        }

        return `${this.formatValue(value)} months`;
    }

    private formatNumberValue(value: number | null | undefined): string {
        if (value === null || value === undefined) {
            return '-';
        }

        return this.formatNumber(value);
    }

    private formatNumber(value: number): string {
        return new Intl.NumberFormat('en-US', {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2
        }).format(Number(value) || 0);
    }
}
