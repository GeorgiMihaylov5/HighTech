using HighTech.Core.Entities;
using HighTech.Core.Entities.Enum;
using HighTech.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HighTech.Infrastructure.Repositories
{
	public class OrderRepository : IOrderRepository
	{
		private readonly ApplicationDbContext context;

		public OrderRepository(ApplicationDbContext _context)
		{
			context = _context;
		}

		public Order CreateOrder(DateTime orderedOn, string username)
		{
			var user = context.Users.FirstOrDefault(u => u.UserName == username);

			if (user is null)
			{
				throw new InvalidOperationException($"User with username '{username}' does not exist.");
			}

			var order = new Order()
			{
				OrderedOn = orderedOn,
				CustomerId = user.Id,
				Status = OrderStatus.Pending,
			};

			context.Orders.Add(order);
			context.SaveChanges();

			return order;
		}

		public bool CreateOrderedProduct(string productId, string orderId, decimal price, int count)
		{
			var product = context.Products.FirstOrDefault(x => x.Id == productId);

			if (product is null)
			{
				throw new InvalidOperationException($"Product with ID '{productId}' does not exist.");
			}

			if (product.Quantity < count)
			{
				throw new InvalidOperationException($"Insufficient quantity for product '{productId}'. Available: {product.Quantity}, Requested: {count}");
			}

			// Validate order exists
			var orderExists = context.Orders.Any(o => o.Id == orderId);
			if (!orderExists)
			{
				throw new InvalidOperationException($"Order with ID '{orderId}' does not exist.");
			}

			product.Quantity -= count;

			context.OrderedProducts.Add(new OrderedProduct()
			{
				ProductId = productId,
				OrderId = orderId,
				OrderedPrice = price,
				Count = count
			});

			return context.SaveChanges() != 0;
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
			var user = context.Users.FirstOrDefault(u => u.UserName == username);
			if (user == null) return new List<Order>();

			return context.Orders
			   .Include(o => o.OrderedProducts)
			   .ThenInclude(d => d.Product)
			   .Where(o => o.CustomerId == user.Id)
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
				.Include(o => o.OrderedProducts)
				.ThenInclude(d => d.Product)
				.ToList();
		}
	}
}
