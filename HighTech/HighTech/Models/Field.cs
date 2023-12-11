namespace HighTech.Models
{
    public class Field
    {
        public string Id { get; set; } 
        public TypeCode TypeCode { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
    }
}
