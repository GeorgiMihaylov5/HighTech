namespace HighTech.Core.Entities
{
    public class CategoryField
    {
        public string? CategoryId { get; set; }
        public string? FieldId { get; set; }
        public Category? Category { get; set; }
        public Field? Field { get; set; }
    }
}
