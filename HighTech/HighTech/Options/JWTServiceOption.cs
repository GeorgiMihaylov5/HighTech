namespace HighTech.Options
{
    public class JWTServiceOption
    {
        public string JwtKey { get; set; }
        public string Issuer { get; set; }
        public int ExpiresDays { get; set; }
    }
}
