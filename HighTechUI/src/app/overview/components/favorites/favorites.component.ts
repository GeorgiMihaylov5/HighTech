import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Favorite } from 'src/app/models/favorite.model';
import { Product } from 'src/app/models/product.model';
import { OverviewFacade } from '../../services/overview-facade.service';

@Component({
    selector: 'app-favorites',
    templateUrl: './favorites.component.html',
    styleUrls: ['./favorites.component.css'],
    standalone: false
})
export class FavoritesComponent implements OnInit {
    public favorites: Favorite[] = [];
    public isLoading: boolean = true;

    constructor(private overviewService: OverviewFacade,
        private toastr: ToastrService) {
    }

    public ngOnInit(): void {
        this.loadFavorites();
    }

    public removeFromFavorites(productId: string): void {
        this.overviewService.deleteFavorite(productId).subscribe((removed: boolean) => {
            if (!removed) {
                return;
            }

            this.favorites = this.favorites.filter((favorite: Favorite) => favorite.productId !== productId);
            this.toastr.success('Product was removed from favorites.');
        });
    }

    public clearFavorites(): void {
        this.overviewService.clearFavorites().subscribe((cleared: boolean) => {
            if (!cleared) {
                return;
            }

            this.favorites = [];
            this.toastr.success('Favorites were cleared.');
        });
    }

    public addToBasket(product: Product): void {
        if (product == null || product.quantity <= 0) {
            this.toastr.info('Product is not available.');
            return;
        }

        const addedCount = this.overviewService.addToBasket({
            id: null,
            productId: product.id,
            product: product,
            orderedPrice: product.price,
            count: 1
        });

        if (addedCount === -1) {
            this.toastr.info('Sign in to add products to the shopping cart.');
            return;
        }

        if (addedCount <= 0) {
            this.toastr.info('All available stock is already in the shopping cart.');
            return;
        }

        this.toastr.success('Product was added to the shopping cart!');
    }

    private loadFavorites(): void {
        this.isLoading = true;
        this.overviewService.getFavorites().subscribe((favorites: Favorite[]) => {
            this.favorites = favorites ?? [];
            this.isLoading = false;
        });
    }
}
