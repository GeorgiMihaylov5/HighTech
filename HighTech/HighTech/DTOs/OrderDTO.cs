using HighTech.Models.Enum;
using HighTech.Models;

namespace HighTech.DTOs
{
    public class OrderDTO
    {
        public string Id { get; set; }
        public string OrderedOn { get; set; }
        public virtual UserDTO User { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public virtual ICollection<OrderedProductDTO> OrderedProducts { get; set; }
    }
}
