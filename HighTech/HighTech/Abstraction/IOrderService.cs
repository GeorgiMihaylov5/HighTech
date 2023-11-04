using HighTech.Models;
using HighTech.Models.Enum;

namespace HighTech.Abstraction
{
    public interface IOrderService
    {
        public ICollection<Order> GetOrders();
        public Order GetOrder(string id);
        public bool CreateOrderedProduct(string productId, string orderId, decimal price, int count);
        public bool EditOrderedProduct(string id, int count);
        public bool CreateOrder(DateTime orderedOn, string customerId);
        public bool EditOrder(string id, OrderStatus status, string notes);
    }
}
