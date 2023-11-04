using System.ComponentModel.DataAnnotations.Schema;

namespace HighTech.Models
{
    public class ProductField
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; } 
        public string ProductId { get; set; } 
        public virtual Product Product { get; set; }
        public string FieldId { get; set; } 
        public virtual Field Field { get; set; }
        public string Value { get; set; }
    }
}
