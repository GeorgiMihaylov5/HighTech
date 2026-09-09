using System.ComponentModel.DataAnnotations.Schema;

namespace HighTech.Models
{
    public class ProductFieldValue
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; } 
        public string ProductId { get; set; }
        public virtual Product Product { get; set; }
        public string CategoryFieldId { get; set; }
        public virtual CategoryField CategoryField { get; set; }
        public string Value { get; set; }
    }
}
