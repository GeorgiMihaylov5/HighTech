namespace HighTech.Models
{
    public class CategoryField
    {
        public string? CategoryId { get; set; }
        public virtual Category? Category { get; set; }
        public string? FieldId { get; set; }
        public virtual Field? Field { get; set; }
    }
}
