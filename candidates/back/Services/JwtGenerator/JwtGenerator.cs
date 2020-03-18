using MeetSport.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AiCandidate.Services.JwtGenerator
{
    public class JwtGenerator : IJwtGenerator
    {
        public sealed class CustomClaimTypes
        {
            public static string Id { get; } = "Id";
            public static string Type { get; } = "Type";
        }
        private JwtOptions JwtOptions { get; }

        public JwtGenerator(IOptions<JwtOptions> jwtOptions)
        {
            JwtOptions = jwtOptions.Value;
        }

        public string GenerateToken(ulong userId)
        {
            ICollection<Claim> tokenClaims = new List<Claim>();
            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.UTF8.GetBytes(JwtOptions.IssuerKey);

            tokenClaims.Add(new Claim(CustomClaimTypes.Id, userId.ToString()));
            tokenClaims.Add(new Claim(CustomClaimTypes.Type, "CANDIDATE"));

            SecurityTokenDescriptor securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(tokenClaims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
            string token = jwtSecurityTokenHandler.WriteToken(securityToken);

            return token;
        }
    }
}
