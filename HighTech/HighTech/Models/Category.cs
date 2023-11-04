namespace HighTech.Models
{
    public class Category
    {
        public string CategoryId { get; set; } 
        public string FieldId { get; set; } 
        public virtual Field Field { get; set; }
    }
}
