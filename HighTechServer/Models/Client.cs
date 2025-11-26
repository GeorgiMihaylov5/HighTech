using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HighTech.Models
{
    public class Client
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        [Required]
        [MaxLength(40)]
        public string Address { get; set; }
        public string UserId { get; set; } 
        public virtual AppUser User { get; set; }
    }
}
