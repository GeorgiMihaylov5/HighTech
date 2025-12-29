namespace HighTech.Core.Entities
{
	public class OrderedProduct
	{
		public string? Id { get; set; }
		public string? ProductId { get; set; }
		public Product? Product { get; set; }
		public string? OrderId { get; set; }
		public Order? Order { get; set; }
		public decimal OrderedPrice { get; set; }
		public int Count { get; set; }
	}
}
