import { Component, ElementRef, HostListener, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { OverviewFacade } from '../overview/services/overview-facade.service';
import { CategoryService } from '../services/category.service';
import { Category } from '../models/category.model';

@Component({
	selector: 'app-nav-menu',
	templateUrl: './nav-menu.component.html',
	styleUrls: ['./nav-menu.component.css'],
	standalone: false
})
export class NavMenuComponent implements OnInit {
	isExpanded = false;
	isCategoriesOpen = false;
	categories: Category[] = [];
	itemCount$: Observable<number>;
	favoriteCount$: Observable<number>;
	comparisonCount$: Observable<number>;

	constructor(
		private overviewFacade: OverviewFacade,
		private categoryService: CategoryService,
		private hostRef: ElementRef
	) {
		this.itemCount$ = this.overviewFacade.basketItemCount$;
		this.favoriteCount$ = this.overviewFacade.favoriteItemCount$;
		this.comparisonCount$ = this.overviewFacade.comparisonCount$;
	}

	ngOnInit(): void {
		this.categoryService.getUniqueCategories().subscribe((categories: Category[]) => {
			this.categories = categories;
		});
	}

	collapse(): void {
		this.isExpanded = false;
		this.isCategoriesOpen = false;
	}

	toggle(): void {
		this.isExpanded = !this.isExpanded;
		if (!this.isExpanded) {
			this.isCategoriesOpen = false;
		}
	}

	toggleCategories(event: Event): void {
		event.stopPropagation();
		this.isCategoriesOpen = !this.isCategoriesOpen;
	}

	closeAll(): void {
		this.isCategoriesOpen = false;
		this.isExpanded = false;
	}

	@HostListener('document:click', ['$event'])
	onDocumentClick(event: MouseEvent): void {
		if (!this.hostRef.nativeElement.contains(event.target)) {
			this.isCategoriesOpen = false;
		}
	}
}
