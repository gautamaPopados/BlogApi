using ConsoleApp1.Data;
using ConsoleApp1.Data.Entities;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using WebApplication1.Data.DTO;
using WebApplication1.Exceptions;
using WebApplication1.Services.IServices;

namespace WebApplication1.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _db;

        private string secretKey;

        private static HashSet<string> blacklistedTokens = new HashSet<string>();

        public UserService(AppDbContext db, IConfiguration configuration)
        {
            _db = db;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
        }

        public bool IsUniqueUser(string email)
        {
            var user = _db.Users.FirstOrDefault(x => x.email == email);
            
            if (user == null)
            {
                return true;
            }
            return false;
        }

        public async Task<AuthorizationResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            var user = _db.Users.FirstOrDefault(u => u.email == loginRequestDTO.Email);

            if (user == null)
            {
                throw new BadRequestException("email or password is incorrect");
            }
            else if (!BCrypt.Net.BCrypt.Verify(loginRequestDTO.Password, user.password))
            {
                throw new BadRequestException("email or password is incorrect");
            }

            
            var token = GenerateToken(user);

            AuthorizationResponseDTO loginResponseDTO = new AuthorizationResponseDTO()
            {
                Token = token,
            };
            return loginResponseDTO;
        }

        public async Task Logout(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException();
            }

            blacklistedTokens.Add(token);
        }

        public bool IsTokenBlacklisted(string token)
        {
            System.Diagnostics.Debug.WriteLine("JWt is blacklisted = " + token);
            System.Diagnostics.Debug.WriteLine(blacklistedTokens.Count());

            return blacklistedTokens.Contains(token);
        }

        public async Task<AuthorizationResponseDTO> Register(RegistrationRequestDTO registrationRequestDTO)
        {

            if (!IsUniqueUser(registrationRequestDTO.Email))
            {
                throw new BadRequestException($"Username '{registrationRequestDTO.Email}' is already taken.");
            }

            User user = new User()
            {
                fullName = registrationRequestDTO.FullName,
                email = registrationRequestDTO.Email,
                password = BCrypt.Net.BCrypt.HashPassword(registrationRequestDTO.Password),
                birthDate = registrationRequestDTO.BirthDate,
                gender = registrationRequestDTO.Gender,
                phoneNumber = registrationRequestDTO.PhoneNumber
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            user.password = "";

            var token = GenerateToken(user);
            AuthorizationResponseDTO loginResponseDTO = new AuthorizationResponseDTO()
            {
                Token = token,
            };

            return loginResponseDTO;

        }

        public String GenerateToken (User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

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
    }
}
