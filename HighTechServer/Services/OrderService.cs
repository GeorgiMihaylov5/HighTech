using HighTech.Abstraction;
using HighTech.Data;
using HighTech.DTOs;
using HighTech.Models;
using HighTech.Models.Enum;
using Microsoft.EntityFrameworkCore;

namespace HighTech.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext context;

        public OrderService(ApplicationDbContext _context)
        {
            context = _context;
        }

        public bool TryCreateOrder(OrderDTO orderDto)
        {
            if (orderDto is null
                || orderDto.OrderedProducts is null
                || orderDto.OrderedProducts.Count == 0
                || string.IsNullOrWhiteSpace(orderDto.Username)
                || string.IsNullOrWhiteSpace(orderDto.City)
                || string.IsNullOrWhiteSpace(orderDto.PostalCode)
                || string.IsNullOrWhiteSpace(orderDto.DeliveryAddress)
                || string.IsNullOrWhiteSpace(orderDto.PhoneNumber))
            {
                return false;
            }

            using var transaction = context.Database.BeginTransaction();

            try
            {
                var user = context.Users.FirstOrDefault(u => u.UserName == orderDto.Username);

                if (user is null)
                {
                    return false;
                }

                var requestedProducts = orderDto.OrderedProducts
                    .GroupBy(op => new { op.ProductId, op.BuildId })
                    .Select(group => new
                    {
                        group.Key.ProductId,
                        group.Key.BuildId,
                        Count = group.Sum(op => op.Count),
                        OrderedPrice = group.First().OrderedPrice
                    })
                    .ToList();

                var productIds = requestedProducts.Select(request => request.ProductId).ToList();
                var products = context.Products
                    .Where(product => productIds.Contains(product.Id))
                    .ToDictionary(product => product.Id);

                var totalDemandByProductId = requestedProducts
                    .GroupBy(r => r.ProductId)
                    .ToDictionary(g => g.Key, g => g.Sum(r => r.Count));

                var hasInvalidRequest = requestedProducts.Any(request =>
                    string.IsNullOrWhiteSpace(request.ProductId)
                    || request.Count <= 0
                    || !products.TryGetValue(request.ProductId, out var product)
                    || product.Quantity < totalDemandByProductId[request.ProductId]);

                if (hasInvalidRequest)
                {
                    return false;
                }

                var order = new Order()
                {
                    OrderedOn = DateTime.UtcNow,
                    CustomerId = user.Id,
                    Status = OrderStatus.Pending,
                    City = orderDto.City.Trim(),
                    PostalCode = orderDto.PostalCode.Trim(),
                    DeliveryAddress = orderDto.DeliveryAddress.Trim(),
                    PhoneNumber = orderDto.PhoneNumber.Trim(),
                    PaymentMethod = PaymentMethod.OnDelivery,
                };

                context.Orders.Add(order);

                var deductedProductIds = new HashSet<string>();

                foreach (var request in requestedProducts)
                {
                    var product = products[request.ProductId];

                    if (deductedProductIds.Add(request.ProductId))
                    {
                        product.Quantity -= totalDemandByProductId[request.ProductId];
                    }

                    context.OrderedProducts.Add(new OrderedProduct()
                    {
                        ProductId = request.ProductId,
                        Order = order,
                        OrderedPrice = request.OrderedPrice,
                        Count = request.Count,
                        BuildId = request.BuildId
                    });
                }

                var result = context.SaveChanges() != 0;
                transaction.Commit();

                return result;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public bool CreateOrderedProduct(string productId, string orderId, decimal price, int count)
        {
            using var transaction = context.Database.BeginTransaction();

            try
            {
                var product = context.Products.FirstOrDefault(x => x.Id == productId);

                if (product is null || product.Quantity < count)
                {
                    return false;
                }
                product.Quantity -= count;

                context.OrderedProducts.Add(new OrderedProduct()
                {
                    ProductId = productId,
                    OrderId = orderId,
                    OrderedPrice = price,
                    Count = count
                });

                var result = context.SaveChanges() != 0;
                transaction.Commit();
                return result;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public bool EditOrder(string id, OrderStatus status, string notes)
        {
            var order = context.Orders.FirstOrDefault(o => o.Id == id);

            if (order is null)
            {
                return false;
            }

            order.Status = status;
            order.Notes = notes;

            return context.SaveChanges() != 0;
        }

        public bool EditOrderedProduct(string id, int count)
        {
            var orderedProduct = context.OrderedProducts.FirstOrDefault(o => o.Id == id);

            if (orderedProduct is null)
            {
                return false;
            }

            orderedProduct.Count = count;

            return context.SaveChanges() != 0;
        }

        public ICollection<Order> GetMyOrders(string username)
        {
            return context.Orders
               .Include(o => o.Customer)
               .Include(o => o.OrderedProducts)
               .ThenInclude(d => d.Product)
               .Where(o => o.Customer.UserName == username)
               .ToList();
        }

        public Order GetOrder(string id)
        {
            return context.Orders
                .Include(o => o.OrderedProducts)
                .FirstOrDefault(o => o.Id == id);
        }

        public ICollection<Order> GetOrders()
        {
            return context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderedProducts)
                .ThenInclude(d => d.Product)
                .ToList();
        }
    }
}
