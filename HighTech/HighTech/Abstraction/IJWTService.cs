using HighTech.Models;

namespace HighTech.Abstraction
{
    public interface IJWTService
    {
        public string CreateJWT(AppUser user, IList<string> roles);
    }
}
