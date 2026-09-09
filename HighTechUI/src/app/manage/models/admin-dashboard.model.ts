export interface AdminDashboard {
    totalOrders: number;
    totalRevenue: number;
    pendingOrders: number;
    totalProducts: number;
    lowStockProducts: number;
    clientsCount: number;
    employeesCount: number;
    reviewsCount: number;
    favoritesCount: number;
    ordersByStatus: DashboardStatusCount[];
    recentSales: DashboardSalesPoint[];
    bestSellingProducts: DashboardProductStat[];
    topRatedProducts: DashboardProductRating[];
    lowStockItems: DashboardLowStockProduct[];
    recentOrders: DashboardRecentOrder[];
}

export interface DashboardStatusCount {
    status: number;
    statusName: string;
    count: number;
}

export interface DashboardSalesPoint {
    date: string;
    orders: number;
    revenue: number;
}

export interface DashboardProductStat {
    productId: string;
    manufacturer: string;
    model: string;
    quantitySold: number;
    revenue: number;
}

export interface DashboardProductRating {
    productId: string;
    manufacturer: string;
    model: string;
    averageRating: number;
    reviewCount: number;
}

export interface DashboardLowStockProduct {
    productId: string;
    manufacturer: string;
    model: string;
    quantity: number;
}

export interface DashboardRecentOrder {
    id: string;
    orderedOn: string;
    status: number;
    statusName: string;
    customerName: string;
    customerEmail: string;
    total: number;
}
