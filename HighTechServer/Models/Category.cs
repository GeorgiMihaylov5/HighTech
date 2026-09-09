using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace HighTech.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class Category
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<CategoryField> CategoryFields { get; set; }
    }
}
