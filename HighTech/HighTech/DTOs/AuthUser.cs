namespace HighTech.DTOs
{
    public class AuthUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Jwt { get; set; }
        public long Exp { get; set; }
        public string Email { get; set; }
        public ICollection<string> Role { get; set; }
    }
}
