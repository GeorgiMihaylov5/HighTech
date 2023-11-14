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
    public isLoading: boolean = false;

    constructor(private overviewFacade: OverviewFacade) {

    }
    ngOnInit(): void {
        this.isLoading = true;

        this.overviewFacade.loadProducts().subscribe((products: Product[]) => {
            this.isLoading = false;
            this.products = products;
        })
    }

}
