using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HighTech.Models
{
    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? Id { get; set; }
        [Required]
        [MaxLength(30)]
        public string? JobTitle { get; set; }
        public string? UserId { get; set; }
        public virtual AppUser? User { get; set; }
    }
}
