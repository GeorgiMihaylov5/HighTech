import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Product } from 'src/app/models/product.model';
import { Review } from 'src/app/models/review.model';
import { IToken } from 'src/api-authorization/models/token.model';
import { AuthorizeService } from 'src/api-authorization/services/authorize-facade.service';
import { OverviewFacade } from '../../../services/overview-facade.service';

@Component({
	selector: 'app-product-reviews',
	templateUrl: './product-reviews.component.html',
	styleUrls: ['./product-reviews.component.css'],
	standalone: false
})
export class ProductReviewsComponent implements OnInit, OnChanges {
	@Input() public product: Product;
	@Output() public productChange = new EventEmitter<Product>();

	public currentUsername: string | null = null;
	public isAuthenticated: boolean = false;
	public reviewRating: number = 5;
	public reviewComment: string = '';
	public isReviewSaving: boolean = false;
	public isReviewRemoving: boolean = false;
	public isReviewDialogOpen: boolean = false;

	constructor(
		private overviewService: OverviewFacade,
		private authService: AuthorizeService,
		private toastr: ToastrService
	) { }

	public ngOnInit(): void {
		this.authService.getTokenData().subscribe((token: IToken | null) => {
			this.currentUsername = token?.nameid ?? null;
			this.isAuthenticated = token != null;
			this.syncReviewForm();
		});
	}

	public ngOnChanges(changes: SimpleChanges): void {
		if (changes.product) {
			this.syncReviewForm();
		}
	}

	public get reviews(): Review[] {
		return this.product?.reviews ?? [];
	}

	public get hasReviews(): boolean {
		return this.reviews.length > 0;
	}

	public get ratingStars(): boolean[] {
		const averageRating = Math.round(this.product?.averageRating ?? 0);
		return [1, 2, 3, 4, 5].map((star: number) => star <= averageRating);
	}

	public get ratingBreakdown(): { rating: number, count: number, percent: number }[] {
		return [5, 4, 3, 2, 1].map((rating: number) => {
			const count = this.reviews.filter((review: Review) => Number(review.rating) === rating).length;
			const percent = this.reviews.length === 0 ? 0 : Math.round((count / this.reviews.length) * 100);

			return { rating, count, percent };
		});
	}

	public get currentUserReview(): Review | null {
		if (this.currentUsername == null) {
			return null;
		}

		return this.reviews.find((review: Review) => review.username === this.currentUsername) ?? null;
	}

	public get canSaveReview(): boolean {
		return this.product != null
			&& this.isAuthenticated
			&& this.currentUsername != null
			&& this.reviewRating >= 1
			&& this.reviewRating <= 5
			&& !this.isReviewSaving
			&& !this.isReviewRemoving;
	}

	public getRatingBarClass(rating: number): string {
		return `rating-fill-${rating}`;
	}

	public saveReview(): void {
		if (!this.canSaveReview) {
			return;
		}

		const isEdit = this.currentUserReview != null;

		this.isReviewSaving = true;
		this.overviewService.upsertReview({
			productId: this.product.id,
			username: this.currentUsername,
			rating: Number(this.reviewRating),
			comment: this.reviewComment
		}).subscribe({
			next: (review: Review) => {
				this.applyReview(review);
				this.syncReviewForm();
				this.isReviewSaving = false;
				this.closeReviewDialog();
				this.toastr.success(isEdit ? 'Review was updated!' : 'Review was added!');
			},
			error: () => {
				this.isReviewSaving = false;
			}
		});
	}

	public openReviewDialog(): void {
		this.syncReviewForm();
		this.isReviewDialogOpen = true;
	}

	public closeReviewDialog(): void {
		if (this.isReviewSaving) {
			return;
		}

		this.isReviewDialogOpen = false;
	}

	public removeReview(): void {
		const review = this.currentUserReview;

		if (review == null || this.product == null || this.currentUsername == null || this.isReviewRemoving) {
			return;
		}

		this.isReviewRemoving = true;
		this.overviewService.deleteReview(this.product.id, this.currentUsername).subscribe({
			next: () => {
				this.removeReviewFromProduct(review.id);
				this.syncReviewForm();
				this.isReviewRemoving = false;
				this.toastr.success('Review was removed!');
			},
			error: () => {
				this.isReviewRemoving = false;
			}
		});
	}

	public getReviewAuthor(review: Review): string {
		const fullName = [review.firstName, review.lastName]
			.filter((value: string) => this.hasText(value))
			.join(' ');

		return fullName || review.username || 'Client';
	}

	private syncReviewForm(): void {
		const review = this.currentUserReview;

		if (review == null) {
			this.reviewRating = 5;
			this.reviewComment = '';
			return;
		}

		this.reviewRating = review.rating;
		this.reviewComment = review.comment ?? '';
	}

	private applyReview(review: Review): void {
		const reviews = [...this.reviews];
		const existingIndex = reviews.findIndex((item: Review) => item.id === review.id);

		if (existingIndex >= 0) {
			reviews[existingIndex] = review;
		}
		else {
			reviews.unshift(review);
		}

		this.updateProductReviews(reviews.sort((a: Review, b: Review) =>
			new Date(b.updatedOn).getTime() - new Date(a.updatedOn).getTime()));
	}

	private removeReviewFromProduct(reviewId: string): void {
		this.updateProductReviews(this.reviews.filter((review: Review) => review.id !== reviewId));
	}

	private updateProductReviews(reviews: Review[]): void {
		const updatedProduct: Product = {
			...this.product,
			reviews: reviews,
			reviewCount: reviews.length,
			averageRating: this.calculateAverageRating(reviews)
		};

		this.product = updatedProduct;
		this.overviewService.syncProductState(updatedProduct);
		this.productChange.emit(updatedProduct);
	}

	private calculateAverageRating(reviews: Review[]): number {
		if (reviews.length === 0) {
			return 0;
		}

		const total = reviews.reduce((sum: number, review: Review) => sum + Number(review.rating), 0);
		return Math.round((total / reviews.length) * 100) / 100;
	}

	private hasText(value: string): boolean {
		return value !== null && value !== undefined && value.trim() !== '';
	}
}
