namespace HighTech.Models
{
    public class Field
    {
        public string? Id { get; set; }
        public TypeCode TypeCode { get; set; }
        public virtual ICollection<CategoryField>? CategoryFields { get; set; }
        public virtual ICollection<ProductField>? ProductsFields { get; set; }

    }
}
