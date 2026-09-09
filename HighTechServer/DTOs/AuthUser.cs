namespace HighTech.DTOs
{
    public class AuthUser
    {
        public string Given_name { get; set; }
        public string Family_name { get; set; }
        public string Jwt { get; set; }
        public long Exp { get; set; }
        public string Nameid { get; set; }
        public ICollection<string> Role { get; set; }
    }
}
