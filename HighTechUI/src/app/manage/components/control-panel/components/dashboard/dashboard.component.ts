import { Component, OnInit } from '@angular/core';
import { AdminDashboard, DashboardRecentOrder, DashboardSalesPoint } from 'src/app/manage/models/admin-dashboard.model';
import { ManageServiceFacade } from 'src/app/manage/services/manage-facade.service';

@Component({
    selector: 'app-admin-dashboard',
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.css'],
    standalone: false
})
export class DashboardComponent implements OnInit {
    public dashboard: AdminDashboard | null = null;
    public isLoading = true;

    constructor(private manageService: ManageServiceFacade) {
    }

    public ngOnInit(): void {
        this.manageService.getDashboard().subscribe({
            next: (dashboard: AdminDashboard) => {
                this.dashboard = dashboard;
                this.isLoading = false;
            },
            error: () => {
                this.isLoading = false;
            }
        });
    }

    public getSalesMax(): number {
        return Math.max(...(this.dashboard?.recentSales ?? []).map(point => point.revenue), 1);
    }

    public getSalesHeight(point: DashboardSalesPoint): number {
        return Math.max(6, Math.round((point.revenue / this.getSalesMax()) * 100));
    }

    public getOrderDate(order: DashboardRecentOrder): string {
        const epochTicks = 621355968000000000;
        const ticksPerMillisecond = 10000;
        const ticksSinceEpoch = parseInt(order.orderedOn) - epochTicks;
        const date = new Date(ticksSinceEpoch / ticksPerMillisecond);

        if (Number.isNaN(date.getTime())) {
            return '-';
        }

        return date.toLocaleDateString('en-US', {
            month: 'short',
            day: 'numeric',
            year: 'numeric'
        });
    }

    public getShortId(id: string): string {
        return `HT-${(id ?? '').replace(/-/g, '').slice(-5).toUpperCase() || '00000'}`;
    }

    public statusToClass(statusName: string): string {
        return `is-${(statusName ?? '').toLowerCase()}`;
    }
}
