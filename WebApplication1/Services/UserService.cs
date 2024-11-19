using ConsoleApp1.Data;
using ConsoleApp1.Data.Entities;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
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

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            var user = _db.Users.FirstOrDefault(u => u.email == loginRequestDTO.email);

            if (user == null)
            {
                throw new BadRequestException("email or password is incorrect");
            }
            else if (!BCrypt.Net.BCrypt.Verify(loginRequestDTO.password, user.password))
            {
                throw new BadRequestException("email or password is incorrect");
            }

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
            LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
            {
                token = tokenHandler.WriteToken(token),
            };
            return loginResponseDTO;
        }

        public async Task<LoginResponseDTO> Register(RegistrationRequestDTO registrationRequestDTO)
        {
            User user = new User()
            {
                fullName = registrationRequestDTO.fullName,
                email = registrationRequestDTO.email,
                password = BCrypt.Net.BCrypt.HashPassword(registrationRequestDTO.password),
                birthDate = registrationRequestDTO.birthDate,
                gender = registrationRequestDTO.gender,
                phoneNumber = registrationRequestDTO.phoneNumber
            };

            _db.Users.Add(user);
            _db.SaveChangesAsync();
            user.password = "";

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
            LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
            {
                token = tokenHandler.WriteToken(token),
            };
            return loginResponseDTO;

        }
    }
}
