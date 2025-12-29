using HighTech.Core.Entities.Enum;

namespace HighTech.Core.Entities
{
	public class Order
	{
		public string? Id { get; set; }
		public DateTime OrderedOn { get; set; }
		public string? CustomerId { get; set; } 
		public AppUser? Customer { get; set; }
		public OrderStatus Status { get; set; }
		public string? Notes { get; set; }
		public ICollection<OrderedProduct>? OrderedProducts { get; set; }
	}
}

