namespace HighTech.Core.Entities
{
	public class Product
	{
		public string? Id { get; set; }
		public string? Manufacturer { get; set; }
		public string? Model { get; set; }
		public int Warranty { get; set; }
		public decimal Price { get; set; }
		public decimal Discount { get; set; }
		public int Quantity { get; set; }
		public bool IsRemoved { get; set; }
		public string? Image { get; set; }
		public string? CategoryID { get; set; }
		public Category? Category { get; set; }
		public ICollection<ProductFieldValue>? ProductFieldValues { get; set; }
	}
}
