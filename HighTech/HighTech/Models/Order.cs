using HighTech.Models.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace HighTech.Models
{
    public class Order
    {
        //TODO
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? Id { get; set; }
        public DateTime OrderedOn { get; set; }
        public string? ProductId { get; set; }
        public virtual Product? Product { get; set; }
        public string? CustomerId { get; set; }
        public virtual AppUser? Customer { get; set; }
        public decimal OrderedPrice { get; set; }
        public int Count { get; set; }
        public OrderStatus Status { get; set; }
        public string? Notes { get; set; }
    }
}
