namespace HighTech.Core.Entities
{
	public class Category
	{
		public string? Id { get; set; }
		public string? Name { get; set; }
		public string? FieldId { get; set; }
		public Field? Field { get; set; }
		public ICollection<ProductCategory>? ProductFields { get; set; }
	}
}
