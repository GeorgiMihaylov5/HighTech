using HighTech.Core.Entities;
using HighTech.Core.Entities.Enum;
using HighTech.Core.Repositories;
using HighTech.Core.Services.Abstraction;

namespace HighTech.Core.Services
{
	public class OrderService : IOrderService
	{
		private readonly IOrderRepository orderRepository;
		private readonly IProductRepository productRepository;

		public OrderService(IOrderRepository _orderRepository, IProductRepository _productRepository)
		{
			orderRepository = _orderRepository;
			productRepository = _productRepository;
		}

		public Order CreateOrder(DateTime orderedOn, string? username)
		{
			// Business validation
			if (string.IsNullOrWhiteSpace(username))
			{
				throw new ArgumentException("Username is required.", nameof(username));
			}

			if (orderedOn > DateTime.UtcNow)
			{
				throw new InvalidOperationException("Order date cannot be in the future.");
			}

			return orderRepository.CreateOrder(orderedOn, username);
		}

		public bool CreateOrderedProduct(string? productId, string? orderId, decimal price, int count)
		{
			// Business validation
			if (string.IsNullOrWhiteSpace(productId))
			{
				throw new ArgumentException("Product ID is required.", nameof(productId));
			}

			if (string.IsNullOrWhiteSpace(orderId))
			{
				throw new ArgumentException("Order ID is required.", nameof(orderId));
			}

			if (count <= 0)
			{
				throw new InvalidOperationException("Count must be greater than zero.");
			}

			if (price < 0)
			{
				throw new InvalidOperationException("Price cannot be negative.");
			}

			// Check product availability
			var product = productRepository.Get(productId);
			if (product == null)
			{
				throw new InvalidOperationException($"Product with ID '{productId}' does not exist.");
			}

			if (product.Quantity < count)
			{
				throw new InvalidOperationException(
					$"Insufficient stock for product '{product.Manufacturer} {product.Model}'. Available: {product.Quantity}, Requested: {count}");
			}

			// Verify order exists
			var order = orderRepository.GetOrder(orderId);
			if (order == null)
			{
				throw new InvalidOperationException($"Order with ID '{orderId}' does not exist.");
			}

			// Only allow adding products to pending or approved orders
			if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Approved)
			{
				throw new InvalidOperationException($"Cannot add products to order with status '{order.Status}'. Only pending or approved orders can be modified.");
			}

			return orderRepository.CreateOrderedProduct(productId, orderId, price, count);
		}

		public bool EditOrder(string? id, OrderStatus status, string? notes)
		{
			if (string.IsNullOrWhiteSpace(id))
			{
				throw new ArgumentException("Order ID is required.", nameof(id));
			}

			// Validate status transition
			var order = orderRepository.GetOrder(id);
			if (order == null)
			{
				throw new InvalidOperationException($"Order with ID '{id}' does not exist.");
			}

			if (!ValidateStatusTransition(order.Status, status))
			{
				throw new InvalidOperationException($"Invalid status transition from '{order.Status}' to '{status}'.");
			}

			return orderRepository.EditOrder(id, status, notes);
		}

		public bool EditOrderedProduct(string? id, int count)
		{
			if (count <= 0)
			{
				throw new InvalidOperationException("Count must be greater than zero.");
			}

			return orderRepository.EditOrderedProduct(id, count);
		}

		public ICollection<Order> GetMyOrders(string? username)
		{
			if (string.IsNullOrWhiteSpace(username))
			{
				throw new ArgumentException("Username is required.", nameof(username));
			}

			return orderRepository.GetMyOrders(username);
		}

		public Order? GetOrder(string? id)
		{
			if (string.IsNullOrWhiteSpace(id))
			{
				throw new ArgumentException("Order ID is required.", nameof(id));
			}

			return orderRepository.GetOrder(id);
		}

		public ICollection<Order> GetOrders()
		{
			return orderRepository.GetOrders();
		}

		public bool CancelOrder(string? id)
		{
			if (string.IsNullOrWhiteSpace(id))
			{
				throw new ArgumentException("Order ID is required.", nameof(id));
			}

			var order = orderRepository.GetOrder(id);
			if (order == null)
			{
				throw new InvalidOperationException($"Order with ID '{id}' does not exist.");
			}

			// Only pending or approved orders can be rejected/cancelled
			if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Approved)
			{
				throw new InvalidOperationException($"Cannot cancel order with status '{order.Status}'.");
			}

			// Restore inventory for all products in the order
			if (order.OrderedProducts != null)
			{
				foreach (var orderedProduct in order.OrderedProducts)
				{
					var product = productRepository.Get(orderedProduct.ProductId!);
					if (product != null)
					{
						// This would ideally call a repository method to update quantity
						// For now, we rely on the repository to handle this
					}
				}
			}

			return orderRepository.EditOrder(id, OrderStatus.Rejected, "Order cancelled by user");
		}

		public decimal CalculateOrderTotal(string? orderId)
		{
			if (string.IsNullOrWhiteSpace(orderId))
			{
				throw new ArgumentException("Order ID is required.", nameof(orderId));
			}

			var order = orderRepository.GetOrder(orderId);
			if (order == null)
			{
				throw new InvalidOperationException($"Order with ID '{orderId}' does not exist.");
			}

			if (order.OrderedProducts == null || !order.OrderedProducts.Any())
			{
				return 0;
			}

			return order.OrderedProducts.Sum(op => op.OrderedPrice * op.Count);
		}

		public bool ValidateStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
		{
			// Define valid status transitions based on actual enum values:
			// Pending -> Approved or Rejected
			// Approved -> Completed or Rejected
			return (currentStatus, newStatus) switch
			{
				// From Pending
				(OrderStatus.Pending, OrderStatus.Approved) => true,
				(OrderStatus.Pending, OrderStatus.Rejected) => true,

				// From Approved
				(OrderStatus.Approved, OrderStatus.Completed) => true,
				(OrderStatus.Approved, OrderStatus.Rejected) => true,

				// Same status is always allowed (no-op)
				var (current, next) when current == next => true,

				// All other transitions are invalid
				_ => false
			};
		}
	}
}
