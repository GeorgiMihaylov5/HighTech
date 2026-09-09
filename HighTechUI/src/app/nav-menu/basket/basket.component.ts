import { Component } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { IToken } from 'src/api-authorization/models/token.model';
import { AuthorizeService } from 'src/api-authorization/services/authorize-facade.service';
import { Order, OrderedProduct, PaymentMethod } from 'src/app/models/order.model';
import { OverviewFacade } from 'src/app/overview/services/overview-facade.service';

@Component({
    selector: 'app-basket',
    templateUrl: './basket.component.html',
    styleUrls: ['./basket.component.css'],
    standalone: false
})
export class BasketComponent {
    public orders: OrderedProduct[];
    public token: IToken;
    public totalPrice: number = 0;
    public expandedBuilds: Set<string> = new Set();
    public checkoutDetails = {
        city: '',
        postalCode: '',
        deliveryAddress: '',
        phoneNumber: ''
    };

    constructor(private overviewService: OverviewFacade,
        private router: Router,
        private toastr: ToastrService,
        private authService: AuthorizeService) {
        this.reloadBasket();
        authService.getTokenData().subscribe((t: IToken) => {
            this.token = t;
            this.prefillCheckoutDetails();
        });
        authService.authorizationChange.subscribe(() => {
            this.reloadBasket();
            authService.getTokenData().subscribe((t: IToken) => {
                this.token = t;
                this.prefillCheckoutDetails();
            });
        });
    }

    public get groupedBasket() {
        const basket = this.orders ?? [];
        const regularProducts = basket.filter(op => !op.buildId);
        const buildMap = new Map<string, OrderedProduct[]>();

        for (const op of basket) {
            if (!op.buildId) {
                continue;
            }

            const existing = buildMap.get(op.buildId) ?? [];
            existing.push(op);
            buildMap.set(op.buildId, existing);
        }

        const builds = Array.from(buildMap.entries()).map(([buildId, items]) => ({
            buildId,
            items,
            total: items.reduce((sum, item) => sum + (item.orderedPrice ?? item.product?.price ?? 0) * item.count, 0)
        }));

        return { items: regularProducts, builds };
    }

    public get regularProducts(): OrderedProduct[] {
        return this.groupedBasket.items;
    }

    public get builds() {
        return this.groupedBasket.builds;
    }

    public toggleBuildExpanded(buildId: string): void {
        if (this.expandedBuilds.has(buildId)) {
            this.expandedBuilds.delete(buildId);
        } else {
            this.expandedBuilds.add(buildId);
        }
    }

    public isBuildExpanded(buildId: string): boolean {
        return this.expandedBuilds.has(buildId);
    }

    public removeBuild(buildId: string): void {
        this.orders = this.overviewService.removeBuild(buildId);
        this.calculateTotalPrice();
    }

    public createOrder(form: NgForm) {
        if (this.token == null) {
            this.router.navigateByUrl('authentication/login');

            return;
        }

        if (this.orders == null || this.orders?.length === 0) {
            this.toastr.info('The shopping cart is empy!');
            return;
        }

        if (form.invalid) {
            this.toastr.info('Please fill in the checkout details before creating the order.');
            return;
        }

        const order: Order = {
            id: null,
            orderedOn: new Date().getTime().toString(),
            user: null,
            username: this.token.nameid,
            status: 0,
            notes: null,
            city: this.checkoutDetails.city,
            postalCode: this.checkoutDetails.postalCode,
            deliveryAddress: this.checkoutDetails.deliveryAddress,
            phoneNumber: this.checkoutDetails.phoneNumber,
            paymentMethod: PaymentMethod.OnDelivery,
            orderedProducts: this.orders
        };

        this.overviewService.createOrder(order).subscribe(_ => {
            this.clean();
            form.resetForm({
                city: '',
                postalCode: '',
                deliveryAddress: '',
                phoneNumber: ''
            });
            this.checkoutDetails = {
                city: '',
                postalCode: '',
                deliveryAddress: '',
                phoneNumber: ''
            };

            this.toastr.success('The order was successful!');

        });
    }

    public get itemCount(): number {
        return (this.orders ?? []).reduce((total: number, order: OrderedProduct) => total + (Number(order.count) || 0), 0);
    }

    public getLineTotal(order: OrderedProduct): number {
        return (Number(order?.product?.price) || 0) * (Number(order?.count) || 0);
    }

    public getBuildItemCount(build: { items: OrderedProduct[] }): number {
        return build.items.reduce((sum, item) => sum + item.count, 0);
    }

    public getBuildCaseImage(build: { items: OrderedProduct[] }): string | null {
        return this.getBuildCaseItem(build)?.product?.image ?? null;
    }

    public getBuildCaseAlt(build: { items: OrderedProduct[] }): string {
        const pcCase = this.getBuildCaseItem(build)?.product;

        return pcCase == null ? 'PC Build' : `${pcCase.manufacturer} ${pcCase.model}`;
    }

    public getBuildCaseItem(build: { items: OrderedProduct[] }): OrderedProduct | null {
        return build.items.find(item => item.product?.categoryName?.toLowerCase() === 'case') ?? null;
    }

    public increaseCount(index: number): void {
        const order = this.regularProducts?.[index];

        if (order == null) {
            return;
        }

        const flatIndex = this.orders.indexOf(order);
        if (flatIndex < 0) return;

        this.orders[flatIndex].count = Math.min(Number(order.product?.quantity) || 0, (Number(order.count) || 1) + 1);
        this.updateBasket();
    }

    public decreaseCount(index: number): void {
        const order = this.regularProducts?.[index];

        if (order == null) {
            return;
        }

        const flatIndex = this.orders.indexOf(order);
        if (flatIndex < 0) return;

        this.orders[flatIndex].count = Math.max(1, (Number(order.count) || 1) - 1);
        this.updateBasket();
    }

    public clean() {
        this.orders = this.overviewService.cleanBasket();
        this.calculateTotalPrice();
    }

    public removeAlaCarteItem(index: number) {
        const flatIndex = this.orders.indexOf(this.regularProducts[index]);
        if (flatIndex < 0) return;
        this.orders = this.overviewService.removeFromBasket(flatIndex);
        this.calculateTotalPrice();
    }

    public updateBasket(): void {
        this.orders = this.overviewService.updateBasket(this.orders);
        this.calculateTotalPrice();
    }

    public calculateTotalPrice(): void {
        this.totalPrice = 0;

        if (this.orders != null && this.orders.length > 0) {
            this.orders.forEach((o: OrderedProduct) => {
                this.totalPrice += (o.orderedPrice ?? o.product?.price ?? 0) * o.count;
            });
        }
    }

    private reloadBasket(): void {
        this.orders = this.overviewService.getBasket();
        this.calculateTotalPrice();

        if (this.orders != null && this.orders.length > 0) {
            this.overviewService.refreshBasket().subscribe((refreshed: OrderedProduct[]) => {
                this.orders = refreshed ?? [];
                this.calculateTotalPrice();
            });
        }
    }

    private prefillCheckoutDetails(): void {
        if (this.token == null) {
            this.checkoutDetails = {
                city: '',
                postalCode: '',
                deliveryAddress: '',
                phoneNumber: ''
            };
            return;
        }

        this.overviewService.getCheckoutProfileDefaults().subscribe(defaults => {
            this.checkoutDetails.phoneNumber = defaults.phoneNumber ?? '';
            this.checkoutDetails.deliveryAddress = defaults.deliveryAddress ?? '';
        });
    }
}
