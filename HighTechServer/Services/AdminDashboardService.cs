using HighTech.Abstraction;
using HighTech.Data;
using HighTech.DTOs;
using HighTech.Models.Enum;
using Microsoft.EntityFrameworkCore;

namespace HighTech.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private const int LowStockThreshold = 5;
        private const int RecentDays = 14;
        private readonly ApplicationDbContext context;

        public AdminDashboardService(ApplicationDbContext _context)
        {
            context = _context;
        }

        public AdminDashboardDTO GetDashboard()
        {
            var orderedProducts = context.OrderedProducts
                .Include(op => op.Product)
                .Include(op => op.Order)
                .ThenInclude(o => o.Customer)
                .ToList();

            var orders = context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderedProducts)
                .ToList();

            var reviews = context.Reviews
                .Include(review => review.Product)
                .Where(review => review.Product != null && review.Product.IsRemoved != true)
                .ToList();

            var today = DateTime.UtcNow.Date;
            var salesStartDate = today.AddDays(-(RecentDays - 1));

            var recentSales = orders
                .Where(order => order.OrderedOn.Date >= salesStartDate)
                .GroupBy(order => order.OrderedOn.Date)
                .Select(group => new DashboardSalesPointDTO
                {
                    Date = group.Key.ToString("yyyy-MM-dd"),
                    Orders = group.Count(),
                    Revenue = group.Sum(order => order.OrderedProducts.Sum(op => op.OrderedPrice * op.Count))
                })
                .ToDictionary(point => point.Date);

            var dashboard = new AdminDashboardDTO
            {
                TotalOrders = orders.Count,
                TotalRevenue = orderedProducts.Sum(op => op.OrderedPrice * op.Count),
                PendingOrders = orders.Count(order => order.Status == OrderStatus.Pending),
                TotalProducts = context.Products.Count(product => product.IsRemoved != true),
                LowStockProducts = context.Products.Count(product => product.IsRemoved != true && product.Quantity <= LowStockThreshold),
                ClientsCount = context.Clients.Count(),
                EmployeesCount = context.Employees.Count(),
                ReviewsCount = context.Reviews.Count(),
                FavoritesCount = context.Favorites.Count(),
                OrdersByStatus = Enum.GetValues<OrderStatus>()
                    .Select(status => new DashboardStatusCountDTO
                    {
                        Status = (int)status,
                        StatusName = status.ToString(),
                        Count = orders.Count(order => order.Status == status)
                    })
                    .ToList(),
                RecentSales = Enumerable.Range(0, RecentDays)
                    .Select(offset => salesStartDate.AddDays(offset).ToString("yyyy-MM-dd"))
                    .Select(date => recentSales.TryGetValue(date, out var point)
                        ? point
                        : new DashboardSalesPointDTO { Date = date, Orders = 0, Revenue = 0 })
                    .ToList(),
                BestSellingProducts = orderedProducts
                    .Where(op => op.Product is not null && op.Product.IsRemoved != true)
                    .GroupBy(op => op.ProductId)
                    .Select(group =>
                    {
                        var product = group.First().Product;
                        return new DashboardProductStatDTO
                        {
                            ProductId = group.Key,
                            Manufacturer = product.Manufacturer,
                            Model = product.Model,
                            QuantitySold = group.Sum(op => op.Count),
                            Revenue = group.Sum(op => op.OrderedPrice * op.Count)
                        };
                    })
                    .OrderByDescending(product => product.QuantitySold)
                    .ThenByDescending(product => product.Revenue)
                    .Take(6)
                    .ToList(),
                TopRatedProducts = reviews
                    .GroupBy(review => review.ProductId)
                    .Select(group => new DashboardProductRatingDTO
                    {
                        ProductId = group.Key,
                        Manufacturer = group.First().Product.Manufacturer,
                        Model = group.First().Product.Model,
                        AverageRating = Math.Round((decimal)group.Average(review => review.Rating), 2),
                        ReviewCount = group.Count()
                    })
                    .OrderByDescending(product => product.AverageRating)
                    .ThenByDescending(product => product.ReviewCount)
                    .Take(6)
                    .ToList(),
                LowStockItems = context.Products
                    .Where(product => product.IsRemoved != true && product.Quantity <= LowStockThreshold)
                    .OrderBy(product => product.Quantity)
                    .ThenBy(product => product.Manufacturer)
                    .Take(8)
                    .Select(product => new DashboardLowStockProductDTO
                    {
                        ProductId = product.Id,
                        Manufacturer = product.Manufacturer,
                        Model = product.Model,
                        Quantity = product.Quantity
                    })
                    .ToList(),
                RecentOrders = orders
                    .OrderByDescending(order => order.OrderedOn)
                    .Take(8)
                    .Select(order => new DashboardRecentOrderDTO
                    {
                        Id = order.Id,
                        OrderedOn = order.OrderedOn.Ticks.ToString(),
                        Status = (int)order.Status,
                        StatusName = order.Status.ToString(),
                        CustomerName = $"{order.Customer?.FirstName} {order.Customer?.LastName}".Trim(),
                        CustomerEmail = order.Customer?.Email,
                        Total = order.OrderedProducts.Sum(op => op.OrderedPrice * op.Count)
                    })
                    .ToList()
            };

            return dashboard;
        }
    }
}
