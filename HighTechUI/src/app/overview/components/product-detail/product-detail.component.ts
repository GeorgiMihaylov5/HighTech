import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { catchError, distinctUntilChanged, map, of, switchMap, tap } from 'rxjs';
import { OverviewFacade } from '../../services/overview-facade.service';
import { Product } from 'src/app/models/product.model';
import { Field } from 'src/app/models/field.model';

@Component({
	selector: 'app-product-detail',
	templateUrl: './product-detail.component.html',
	styleUrls: ['./product-detail.component.css'],
	standalone: false
})
export class ProductDetailComponent implements OnInit {
	public orderedCount: number = 1;
	public product: Product;
	public isLoading: boolean = true;
	public isFavorite: boolean = false;
	public isCompared: boolean = false;

	constructor(private overviewService: OverviewFacade,
		private route: ActivatedRoute,
		private toastr: ToastrService) { }

	public ngOnInit(): void {
		this.route.paramMap.pipe(
			map(params => params.get('id')),
			distinctUntilChanged(),
			tap(() => {
				this.isLoading = true;
				this.orderedCount = 1;
				this.isFavorite = false;
				this.isCompared = false;
			}),
			switchMap((productId: string | null) => {
				if (!productId) {
					return of(null);
				}

				return this.overviewService.getProduct(productId);
			}),
			catchError(() => of(null))
		).subscribe((product: Product | null) => {
			this.product = product;
			this.isLoading = false;

			if (product != null) {
				this.loadFavoriteState(product.id);
				this.isCompared = this.overviewService.isCompared(product.id);
			}
		});
	}

	public makeOrder() {
		this.normalizeOrderedCount();

		const addedCount = this.overviewService.addToBasket({
			id: null,
			productId: this.product.id,
			product: this.product,
			orderedPrice: this.product.price,
			count: this.orderedCount
		});

		if (addedCount === -1) {
			this.toastr.info('Sign in to add products to the basket.');
			return;
		}

		if (addedCount <= 0) {
			this.toastr.info('All available stock is already in the basket.');
			return;
		}

		this.toastr.success(
			addedCount === 1 ? 'Product was added to the basket!' : `${addedCount} products were added to the basket!`
		);
	}

	public increaseQuantity(): void {
		if (!this.product) {
			return;
		}

		this.orderedCount = Math.min(this.product.quantity, this.orderedCount + 1);
	}

	public decreaseQuantity(): void {
		this.orderedCount = Math.max(1, this.orderedCount - 1);
	}

	public normalizeOrderedCount(): void {
		if (!this.product) {
			this.orderedCount = 1;
			return;
		}

		const requestedCount = Math.max(1, Number(this.orderedCount) || 1);
		this.orderedCount = Math.min(requestedCount, Math.max(1, this.product.quantity));
	}

	public toggleFavorite(): void {
		if (!this.product) {
			return;
		}

		if (this.isFavorite) {
			this.overviewService.deleteFavorite(this.product.id).subscribe((removed: boolean) => {
				if (!removed) {
					return;
				}

				this.isFavorite = false;
				this.toastr.success('Product was removed from favorites.');
			});

			return;
		}

		this.overviewService.addFavorite(this.product.id).subscribe((favorite) => {
			if (favorite == null) {
				this.toastr.info('Sign in to save favorites.');
				return;
			}

			this.isFavorite = true;
			this.toastr.success('Product was added to favorites.');
		});
	}

	public toggleCompare(): void {
		if (!this.product) {
			return;
		}

		const isCompared = this.overviewService.toggleCompare(this.product);

		if (isCompared == null) {
			this.toastr.info('You can compare up to 3 products.');
			return;
		}

		this.isCompared = isCompared;
		this.toastr.success(isCompared
			? 'Product was added to comparison.'
			: 'Product was removed from comparison.');
	}

	public get originalSinglePrice(): number {
		if (!this.product) {
			return 0;
		}

		return this.product.price + this.product.discount;
	}

	public get totalPrice(): number {
		return (this.product?.price ?? 0) * this.orderedCount;
	}

	public get originalTotalPrice(): number {
		return this.originalSinglePrice * this.orderedCount;
	}

	public get discountPercentage(): number {
		if (!this.product || this.product.discount <= 0 || this.originalSinglePrice <= 0) {
			return 0;
		}

		return Math.round((this.product.discount / this.originalSinglePrice) * 100);
	}

	public get stockStatusText(): string {
		if (!this.product || this.product.quantity <= 0) {
			return 'Out of stock';
		}

		if (this.product.quantity <= 5) {
			return `Only ${this.product.quantity} left`;
		}

		return 'In stock';
	}

	public get stockStatusClass(): string {
		if (!this.product || this.product.quantity <= 0) {
			return 'is-danger';
		}

		if (this.product.quantity <= 5) {
			return '';
		}

		return 'is-success';
	}

	public get visibleFields(): Field[] {
		return this.product?.fields?.filter((field: Field) => this.hasValue(field)) ?? [];
	}

	public get ratingStars(): boolean[] {
		const averageRating = Math.round(this.product?.averageRating ?? 0);
		return [1, 2, 3, 4, 5].map((star: number) => star <= averageRating);
	}

	public updateProduct(product: Product): void {
		this.product = product;
	}

	private hasValue(field: Field): boolean {
		return field.value !== null && field.value !== undefined && (typeof field.value !== 'string' || field.value.trim() !== '');
	}

	private loadFavoriteState(productId: string): void {
		this.overviewService.isFavorite(productId).subscribe((isFavorite: boolean) => {
			this.isFavorite = isFavorite;
		});
	}
}
