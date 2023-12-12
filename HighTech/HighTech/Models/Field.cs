using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace HighTech.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class Field
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string Name { get; set; }
        public TypeCode TypeCode { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
    }
}
