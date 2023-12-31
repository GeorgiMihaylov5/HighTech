﻿using HighTech.Abstraction;
using HighTech.Models;
using HighTech.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HighTech.Services
{
    public class JWTService : IJWTService
    {
        private readonly SymmetricSecurityKey jwtKey;
        private readonly int expiresDays;
        private readonly string issuer;

        public JWTService(IOptions<JWTServiceOption> options)
        {
            jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.JwtKey!));
            expiresDays = options.Value.ExpiresDays;
            issuer = options.Value.Issuer;
        }

        public int ExpiresDays
            => expiresDays;

        public string CreateJWT(AppUser user, IList<string> roles)
        {
            var userClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.GivenName, user.FirstName!),
                new Claim(ClaimTypes.Surname, user.LastName!),
                new Claim(ClaimTypes.NameIdentifier, user.UserName!),
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
                Expires = DateTime.UtcNow.AddDays(expiresDays),
                SigningCredentials = credentials,
                Issuer = issuer
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwt = tokenHandler.CreateToken(tokenDecriptor);

            return tokenHandler.WriteToken(jwt);
        }
    }
}
