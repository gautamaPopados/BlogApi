using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication1.Data.Entities;
using WebApplication1.Exceptions;

namespace WebApplication1.AuthentificationServices
{
    public class TokenService
    {
        private readonly string _secretKey;
        private readonly JwtSecurityTokenHandler _tokenHandler;

        public TokenService(IConfiguration configuration)
        {
            _secretKey = configuration.GetValue<string>("ApiSettings:Secret");
            _tokenHandler = new JwtSecurityTokenHandler();
        }
        public String GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())

                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public string GetUserId(string token)
        {
            var jwt = _tokenHandler.ReadToken(token) as JwtSecurityToken;

            if (jwt == null)
            {
                throw new UnauthorizedAccessException("Invalid token.");
            }

            var uniqueNameClaim = jwt.Claims.FirstOrDefault(c => c.Type == "unique_name");

            if (uniqueNameClaim == null)
            {
                throw new UnauthorizedAccessException("Id not found.");
            }

            return uniqueNameClaim.Value;
        }
    }
}
