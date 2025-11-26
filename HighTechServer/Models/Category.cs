using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace HighTech.Models
{
    public class Category
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string FieldId { get; set; }
        public virtual Field Field { get; set; }
        public virtual ICollection<ProductCategory> ProductFields { get; set; }
    }
}
