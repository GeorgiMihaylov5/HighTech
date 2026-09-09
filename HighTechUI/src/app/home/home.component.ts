import { Component, OnInit } from '@angular/core';
import { OverviewFacade } from '../overview/services/overview-facade.service';
import { Product } from '../models/product.model';
import { CategoryService } from '../services/category.service';
import { Category } from '../models/category.model';

interface CategoryTile {
	name: string;
	subtitle: string;
	icon: string;
}

interface CategoryPreset {
	matches: string[];
	icon: string;
	subtitle: string;
}

@Component({
	selector: 'app-home',
	templateUrl: './home.component.html',
	styleUrls: ['./home.component.css'],
	standalone: false
})
export class HomeComponent implements OnInit {
	private static readonly MAX_CATEGORY_TILES = 5;

	private static readonly CATEGORY_PRESETS: CategoryPreset[] = [
		{ matches: ['cpu', 'processor'],                                icon: 'bi-cpu',         subtitle: 'High performance CPUs' },
		{ matches: ['gpu', 'graphic', 'video'],                         icon: 'bi-gpu-card',    subtitle: 'Powerful GPUs' },
		{ matches: ['motherboard', 'mainboard'],                        icon: 'bi-motherboard', subtitle: 'Reliable & compatible' },
		{ matches: ['ram', 'memory'],                                   icon: 'bi-memory',      subtitle: 'Fast & reliable RAM' },
		{ matches: ['ssd', 'hdd', 'storage', 'drive'],                  icon: 'bi-device-ssd',  subtitle: 'SSDs & HDDs' }
	];

	private static readonly DEFAULT_PRESET: Omit<CategoryPreset, 'matches'> = {
		icon: 'bi-box-seam',
		subtitle: 'Explore selection'
	};

	public products: Product[] = [];
	public categoryTiles: CategoryTile[] = [];

	constructor(
		private overviewFacade: OverviewFacade,
		private categoryService: CategoryService
	) {
		overviewFacade.getMostSellers().subscribe((products: Product[]) => {
			this.products = products ?? [];
		});
	}

	ngOnInit(): void {
		this.categoryService.getUniqueCategories().subscribe((categories: Category[]) => {
			this.categoryTiles = categories
				.slice(0, HomeComponent.MAX_CATEGORY_TILES)
				.map(category => {
					const preset = this.getCategoryPreset(category.name);
					return {
						name: category.name,
						icon: preset.icon,
						subtitle: preset.subtitle
					};
				});
		});
	}

	public routeToDetail(product: Product): void {
		this.overviewFacade.detailProduct(product);
	}

	public getRatingStars(product: Product): boolean[] {
		const averageRating = Math.round(product?.averageRating ?? 0);
		return [1, 2, 3, 4, 5].map((star: number) => star <= averageRating);
	}

	private getCategoryPreset(name: string): Omit<CategoryPreset, 'matches'> {
		const key = (name ?? '').toLowerCase();

		return HomeComponent.CATEGORY_PRESETS.find(preset =>
			preset.matches.some(match => key.includes(match))
		) ?? HomeComponent.DEFAULT_PRESET;
	}
}
