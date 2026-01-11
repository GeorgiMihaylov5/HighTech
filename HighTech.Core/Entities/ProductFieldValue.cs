namespace HighTech.Core.Entities
{
	public class ProductFieldValue
	{
		public string? ProductID { get; set; }
		public string? FieldID { get; set; }
		public string? Value { get; set; }
		public Product? Product { get; set; }
		public Field? Field { get; set; }
	}
}