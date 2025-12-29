namespace HighTech.Core.Entities
{
	public class ProductCategory
	{
		public string? Id { get; set; }
		public string? ProductId { get; set; }
		public Product? Product { get; set; }
		public string? CategoryId { get; set; }
		public Category? Category { get; set; }
		public string? Value { get; set; }
	}
}
