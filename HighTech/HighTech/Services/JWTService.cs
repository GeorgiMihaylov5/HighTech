using HighTech.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HighTech.Services
{
    public class JWTService
    {
        private readonly IConfiguration config;
        private readonly SymmetricSecurityKey jwtKey;

        public JWTService(IConfiguration _config)
        {
            config = _config;
            jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Key"]));
        }
        public string CreateJWT(ApplicationUser user, IList<string> roles)
        {
            var userClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.GivenName, user.FirstName!),
                new Claim(ClaimTypes.Surname, user.LastName!),
            };

            if (roles is not null)
            {
                foreach (var role in roles)
                {
                    userClaims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            var credentials = new SigningCredentials(jwtKey, SecurityAlgorithms.HmacSha256);
            var tokenDecriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(userClaims),
                Expires = DateTime.UtcNow.AddDays(int.Parse(config["JWT:ExpiresDays"])),
                SigningCredentials = credentials,
                Issuer = config["JWT:Issuer"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwt = tokenHandler.CreateToken(tokenDecriptor);

            return tokenHandler.WriteToken(jwt);
        }
    }
}
