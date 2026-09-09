import { Component, Input, OnInit } from '@angular/core';
import { Observable, catchError } from 'rxjs';
import { IToken } from 'src/api-authorization/models/token.model';
import { Order, OrderedProduct, Status } from 'src/app/models/order.model';
import { OrderService } from 'src/app/services/order.service';
import { AuthorizeService } from 'src/api-authorization/services/authorize-facade.service';
import { Router } from '@angular/router';
import { ErrorService } from 'src/app/services/error.service';
import { groupOrderedProducts } from 'src/app/core/basket-group.util';

@Component({
	selector: 'app-orders',
	templateUrl: './orders.component.html',
	styleUrls: ['./orders.component.css'],
	standalone: false
})
export class OrdersComponent implements OnInit {
	@Input() token: Observable<IToken>;
	public orders: Order[];
	public isStaffView: boolean = false;
	private expandedOrderIds = new Set<string>();
	private static readonly thumbnailLimit = 2;

	constructor(private router: Router,
		private orderApi: OrderService,
		private authService: AuthorizeService,
		private errorService: ErrorService) {
	}

	public ngOnInit(): void {
		this.getOrders(this.router.url);
	}

	public getOrderDate(order: Order): string {
		const epochTicks = 621355968000000000;
		const ticksPerMillisecond = 10000;
		const maxDateMilliseconds = 8640000000000000;

		const ticksSinceEpoch = parseInt(order.orderedOn) - epochTicks;
		const millisecondsSinceEpoch = ticksSinceEpoch / ticksPerMillisecond;

		if (millisecondsSinceEpoch > maxDateMilliseconds) {
			return '—';
		}

		const date = new Date(millisecondsSinceEpoch);
		const datePart = date.toLocaleDateString('en-US', {
			year: 'numeric',
			month: 'long',
			day: 'numeric'
		});
		const timePart = date.toLocaleTimeString('en-US', {
			hour: '2-digit',
			minute: '2-digit'
		});

		return `${datePart} at ${timePart}`;
	}

	public getOrderShortId(order: Order): string {
		const id = order?.id ?? '';
		const trimmed = id.replace(/-/g, '').slice(-5).toUpperCase();
		return `HT-${trimmed || '00000'}`;
	}

	public getOrderSku(orderedProduct: OrderedProduct): string {
		const id = orderedProduct?.productId ?? orderedProduct?.product?.id ?? '';
		const trimmed = id.replace(/-/g, '').slice(-8).toUpperCase();
		return `SKU-${trimmed || 'UNKNOWN'}`;
	}

	public getTotalPrice(order: Order): number {
		return (order?.orderedProducts ?? [])
			.reduce((total, op) => total + op.orderedPrice * op.count, 0);
	}

	public getShipping(_: Order): number {
		return 0;
	}

	public getPaymentMethod(_: Order): string {
		return 'Cash on delivery';
	}

	public getOrderItemCount(order: Order): number {
		return order?.orderedProducts?.length ?? 0;
	}

	public getThumbnailProducts(order: Order): OrderedProduct[] {
		return (order?.orderedProducts ?? []).slice(0, OrdersComponent.thumbnailLimit);
	}

	public getRemainingThumbnailCount(order: Order): number {
		const total = order?.orderedProducts?.length ?? 0;
		return Math.max(0, total - OrdersComponent.thumbnailLimit);
	}

	public getLineTotal(orderedProduct: OrderedProduct): number {
		return orderedProduct.orderedPrice * orderedProduct.count;
	}

	public getGroupedOrderItems(order: Order): { regularProducts: OrderedProduct[], builds: { buildId: string, items: OrderedProduct[], total: number }[] } {
		const { items, builds } = groupOrderedProducts(order?.orderedProducts ?? []);
		return { regularProducts: items, builds };
	}

	public getBuildItemCount(items: OrderedProduct[]): number {
		return items.reduce((sum, item) => sum + item.count, 0);
	}

	public toggleOrderProducts(orderId: string): void {
		if (this.expandedOrderIds.has(orderId)) {
			this.expandedOrderIds.delete(orderId);
			return;
		}

		this.expandedOrderIds.add(orderId);
	}

	public isOrderExpanded(orderId: string): boolean {
		return this.expandedOrderIds.has(orderId);
	}

	public viewProduct(productId: string, event?: Event): void {
		if (event) {
			event.stopPropagation();
		}

		if (productId == null) {
			return;
		}

		this.router.navigate(['/detail', productId]);
	}

	public changeStatus(order: Order) {
		this.orderApi.editStatus(order).subscribe(_ => {

		}, catchError(this.errorService.handleError.bind(this.errorService)));
	}

	public statusToString(status: Status | number | string): string {
		switch (this.normalizeStatus(status)) {
			case Status.Pending:
				return 'Pending';
			case Status.Confirmed:
				return 'Confirmed';
			case Status.Preparing:
				return 'Preparing';
			case Status.Shipped:
				return 'Shipped';
			case Status.Completed:
				return 'Completed';
			case Status.Cancelled:
				return 'Cancelled';
			case Status.Rejected:
				return 'Rejected';
			default:
				return 'Unknown';
		}
	}

	public statusToClass(status: Status | number | string): string {
		switch (this.normalizeStatus(status)) {
			case Status.Pending:
				return 'is-pending';
			case Status.Confirmed:
				return 'is-confirmed';
			case Status.Preparing:
				return 'is-preparing';
			case Status.Shipped:
				return 'is-shipped';
			case Status.Completed:
				return 'is-completed';
			case Status.Cancelled:
				return 'is-cancelled';
			case Status.Rejected:
				return 'is-rejected';
			default:
				return '';
		}
	}

	private normalizeStatus(status: Status | number | string): Status | null {
		if (typeof status === 'number') {
			return status;
		}

		const numericStatus = Number(status);

		if (!Number.isNaN(numericStatus)) {
			return numericStatus;
		}

		const enumStatus = Status[status as keyof typeof Status];

		return typeof enumStatus === 'number' ? enumStatus : null;
	}

	public getStatuses() {
		return Object.entries(Status)
			.map(([key, value]) => ({ key, value }))
			.filter((v) => isNaN(Number(v.value)))
			.map((v) => ({ key: parseInt(v.key), value: v.value }));
	}


	private getOrders(url: string) {
		this.token = this.authService.getTokenData();

		if ((url.includes('/manage/(manage:control-panel'))) {
			this.isStaffView = true;
			this.orderApi.getOrders().subscribe(o => {
				this.orders = o;
			})
		}
		else if (url === '/manage/(manage:orders)') {
			this.isStaffView = false;
			this.token.subscribe(data => {
				this.orderApi.getMyOrders(data.nameid).subscribe(o => {
					this.orders = o;
				})
			});
		}
	}
}
