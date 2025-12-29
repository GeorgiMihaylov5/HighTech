using HighTech.Core.Entities;
using HighTech.Core.Entities.Enum;

namespace HighTech.Core.Repositories
{
	public interface IOrderRepository
	{
		public ICollection<Order> GetOrders();
		public ICollection<Order> GetMyOrders(string username);
		public Order? GetOrder(string id);
		public Order? CreateOrder(DateTime orderedOn, string username);
		public bool CreateOrderedProduct(string productId, string orderId, decimal price, int count);
		public bool EditOrderedProduct(string id, int count);
		public bool EditOrder(string id, OrderStatus status, string notes);
	}
}
