using System.ComponentModel.DataAnnotations.Schema;

namespace HighTech.Models
{
    public class OrderedProduct
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string ProductId { get; set; }
        public virtual Product Product { get; set; }
        public string OrderId { get; set; }
        public virtual Order Order { get; set; }
        public decimal OrderedPrice { get; set; }
        public int Count { get; set; }
    }
}
