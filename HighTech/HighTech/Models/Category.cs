namespace HighTech.Models
{
    public class Category
    {
        public string? CategoryId { get; set; }
        public virtual ICollection<CategoryField>? CategoryFields { get; set; }
    }
}
