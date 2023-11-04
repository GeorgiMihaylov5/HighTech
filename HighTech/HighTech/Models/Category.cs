namespace HighTech.Models
{
    public class Category
    {
        public string Id { get; set; } 
        public virtual ICollection<CategoryField> CategoryFields { get; set; }
    }
}
