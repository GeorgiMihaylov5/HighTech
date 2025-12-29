using HighTech.Core.Entities;
using HighTech.Core.Entities.Enum;
using HighTech.Core.Repositories;
using HighTech.Core.Services.Abstraction;

namespace HighTech.Core.Services
{
	public class OrderService : IOrderService
	{
		private readonly IOrderRepository orderRepository;

		public OrderService(IOrderRepository _orderRepository)
		{
			orderRepository = _orderRepository;
		}

		public Order CreateOrder(DateTime orderedOn, string username)
		{
			return orderRepository.CreateOrder(orderedOn, username);
		}

		public bool CreateOrderedProduct(string productId, string orderId, decimal price, int count)
		{
			return orderRepository.CreateOrderedProduct(productId, orderId, price, count);
		}

		public bool EditOrder(string id, OrderStatus status, string notes)
		{
			return orderRepository.EditOrder(id, status, notes);
		}

		public bool EditOrderedProduct(string id, int count)
		{
			return orderRepository.EditOrderedProduct(id, count);
		}

		public ICollection<Order> GetMyOrders(string username)
		{
			return orderRepository.GetMyOrders(username);
		}

		public Order GetOrder(string id)
		{
			return orderRepository.GetOrder(id);
		}

		public ICollection<Order> GetOrders()
		{
			return orderRepository.GetOrders();
		}
	}
}
