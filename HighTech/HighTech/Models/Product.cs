using System.ComponentModel.DataAnnotations.Schema;

namespace HighTech.Models
{
    public class Product
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? Id { get; set; }
        public string? Manufacturer { get; set; }
        public string? Model { get; set; }
        public int Warranty { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public int Quantity { get; set; }
        public bool IsRemoved { get; set; }
        public string? Image { get; set; }
        public virtual ICollection<ProductField>? ProductFields { get; set; }
    }
}
