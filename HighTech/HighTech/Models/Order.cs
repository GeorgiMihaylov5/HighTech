using HighTech.Models.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace HighTech.Models
{
    public class Order
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? Id { get; set; }
        public DateTime OrderedOn { get; set; }
        public string? CustomerId { get; set; }
        public virtual AppUser? Customer { get; set; }
        public OrderStatus Status { get; set; }
        public string? Notes { get; set; }
        public virtual ICollection<OrderedProduct>? OrderedProducts { get; set; }
    }
}
