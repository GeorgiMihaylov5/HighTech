using Microsoft.AspNetCore.Identity;

namespace HighTech.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }       
    }
}