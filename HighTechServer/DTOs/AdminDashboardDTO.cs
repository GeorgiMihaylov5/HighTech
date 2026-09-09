namespace HighTech.DTOs
{
    public class AdminDashboardDTO
    {
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int PendingOrders { get; set; }
        public int TotalProducts { get; set; }
        public int LowStockProducts { get; set; }
        public int ClientsCount { get; set; }
        public int EmployeesCount { get; set; }
        public int ReviewsCount { get; set; }
        public int FavoritesCount { get; set; }
        public ICollection<DashboardStatusCountDTO> OrdersByStatus { get; set; } = new List<DashboardStatusCountDTO>();
        public ICollection<DashboardSalesPointDTO> RecentSales { get; set; } = new List<DashboardSalesPointDTO>();
        public ICollection<DashboardProductStatDTO> BestSellingProducts { get; set; } = new List<DashboardProductStatDTO>();
        public ICollection<DashboardProductRatingDTO> TopRatedProducts { get; set; } = new List<DashboardProductRatingDTO>();
        public ICollection<DashboardLowStockProductDTO> LowStockItems { get; set; } = new List<DashboardLowStockProductDTO>();
        public ICollection<DashboardRecentOrderDTO> RecentOrders { get; set; } = new List<DashboardRecentOrderDTO>();
    }

    public class DashboardStatusCountDTO
    {
        public int Status { get; set; }
        public string StatusName { get; set; }
        public int Count { get; set; }
    }

    public class DashboardSalesPointDTO
    {
        public string Date { get; set; }
        public int Orders { get; set; }
        public decimal Revenue { get; set; }
    }

    public class DashboardProductStatDTO
    {
        public string ProductId { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public int QuantitySold { get; set; }
        public decimal Revenue { get; set; }
    }

    public class DashboardProductRatingDTO
    {
        public string ProductId { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public decimal AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }

    public class DashboardLowStockProductDTO
    {
        public string ProductId { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public int Quantity { get; set; }
    }

    public class DashboardRecentOrderDTO
    {
        public string Id { get; set; }
        public string OrderedOn { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public decimal Total { get; set; }
    }
}
