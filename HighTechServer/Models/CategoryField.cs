using System.ComponentModel.DataAnnotations.Schema;

namespace HighTech.Models
{
    public class CategoryField
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public string FieldId { get; set; }
        public virtual Field Field { get; set; }
        public virtual ICollection<ProductFieldValue> ProductFields { get; set; }
    }
}
