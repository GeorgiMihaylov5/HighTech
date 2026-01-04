namespace HighTech.Core.Entities
{
	public class Field
	{
		public string? Id { get; set; }
		public string? Name { get; set; }
		public TypeCode TypeCode { get; set; }
		public ICollection<CategoryField>? CategoryFields { get; set; }
        public ICollection<ProductFieldValue>? ProductFieldValues { get; set; }
    }
}
