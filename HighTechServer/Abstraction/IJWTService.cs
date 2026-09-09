using HighTech.Models;

namespace HighTech.Abstraction
{
    public interface IJWTService
    {
        public int ExpiresDays { get; }
        public string CreateJWT(AppUser user, IList<string> roles);
    }
}
