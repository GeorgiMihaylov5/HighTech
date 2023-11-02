using System.ComponentModel.DataAnnotations.Schema;

namespace HighTech.Models
{
    public class Field
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? Id { get; set; }
        public string? Name { get; set; }
        public TypeCode TypeCode { get; set; }
        public virtual ICollection<CategoryField>? CategoryFields { get; set; }
    }
}
