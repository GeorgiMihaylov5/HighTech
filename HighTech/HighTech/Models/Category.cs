namespace HighTech.Models
{
    public class Category
    {
        public string? CategoryId { get; set; }
        public virtual ICollection<Field>? Fields { get; set; }
    }
}
