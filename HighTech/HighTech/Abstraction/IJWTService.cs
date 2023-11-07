using HighTech.Models;

namespace HighTech.Abstraction
{
    public interface IJWTService
    {
        public string CreateJWT(Client user, IList<string> roles);
    }
}
