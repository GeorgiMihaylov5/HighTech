namespace HighTech.Core.Entities
{
	public class Category
	{
		public string? Id { get; set; }
		public string? Name { get; set; }
        public bool IsRemoved { get; set; }
        public ICollection<Product>? Products { get; set; }
        public ICollection<CategoryField>? CategoryFields { get; set; }
    }
}
