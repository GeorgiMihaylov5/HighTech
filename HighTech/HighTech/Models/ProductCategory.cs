using System.ComponentModel.DataAnnotations.Schema;

namespace HighTech.Models
{
    public class ProductCategory
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; } 
        public string ProductId { get; set; }
        public virtual Product Product { get; set; }
        public string CategoryFieldId { get; set; }
        public string CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public string Value { get; set; }
    }
}
