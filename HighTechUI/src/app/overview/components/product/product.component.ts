import { Component, Input, OnInit } from '@angular/core';
import { Product } from '../../../models/product.model';
import { OverviewFacade } from '../../services/overview-facade.service';
import { Field } from 'src/app/models/field.model';
import { ToastrService } from 'ngx-toastr';

@Component({
	selector: 'app-product',
	templateUrl: './product.component.html',
	styleUrls: ['./product.component.css'],
	standalone: false
})
export class ProductComponent implements OnInit {
	@Input() public product: Product;
	public isFavorite: boolean = false;
	public isCompared: boolean = false;

	constructor(private facade: OverviewFacade,
		private toastr: ToastrService) {

	}
	ngOnInit(): void {
		if (this.product?.id) {
			this.loadFavoriteState();
			this.isCompared = this.facade.isCompared(this.product.id);
		}
	}

	public routeToDetail() {
		this.facade.detailProduct(this.product);
	}

	public toggleFavorite(event: Event): void {
		event.stopPropagation();

		if (!this.product?.id) {
			return;
		}

		if (this.isFavorite) {
			this.facade.deleteFavorite(this.product.id).subscribe((removed: boolean) => {
				if (!removed) {
					return;
				}

				this.isFavorite = false;
				this.toastr.success('Product was removed from favorites.');
			});

			return;
		}

		this.facade.addFavorite(this.product.id).subscribe((favorite) => {
			if (favorite == null) {
				this.toastr.info('Sign in to save favorites.');
				return;
			}

			this.isFavorite = true;
			this.toastr.success('Product was added to favorites.');
		});
	}

	public toggleCompare(event: Event): void {
		event.stopPropagation();

		if (!this.product?.id) {
			return;
		}

		const isCompared = this.facade.toggleCompare(this.product);

		if (isCompared == null) {
			this.toastr.info('You can compare up to 3 products.');
			return;
		}

		this.isCompared = isCompared;
		this.toastr.success(isCompared
			? 'Product was added to comparison.'
			: 'Product was removed from comparison.');
	}

	public get ratingStars(): boolean[] {
		const averageRating = Math.round(this.product?.averageRating ?? 0);
		return [1, 2, 3, 4, 5].map((star: number) => star <= averageRating);
	}

	public get description() {
		return this.product.fields
			.filter((field: Field) => this.hasValue(field))
			.sort((a, b) => a.name < b.name ? -1 : a.name > b.name ? 1 : 0)
			.slice(0, 3);
	}

	private hasValue(field: Field): boolean {
		return field.value !== null && field.value !== undefined && (typeof field.value !== 'string' || field.value.trim() !== '');
	}

	private loadFavoriteState(): void {
		this.facade.isFavorite(this.product.id).subscribe((isFavorite: boolean) => {
			this.isFavorite = isFavorite;
		});
	}
}
