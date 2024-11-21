using ConsoleApp1.Data;
using ConsoleApp1.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using WebApplication1.AuthentificationServices;
using WebApplication1.Data.DTO;
using WebApplication1.Exceptions;
using WebApplication1.Services.IServices;

namespace WebApplication1.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _db;
        private readonly TokenService _tokenService;

        public UserService(AppDbContext db, IConfiguration configuration, TokenService tokenService)
        {
            _db = db;
            _tokenService = tokenService;
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

            
            var token = _tokenService.GenerateToken(user);

            AuthorizationResponseDTO loginResponseDTO = new AuthorizationResponseDTO()
            {
                Token = token,
            };
            return loginResponseDTO;
        }

        public Task Logout(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException();
            }

            return Task.CompletedTask;
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
                createTime = DateTime.UtcNow,
                gender = registrationRequestDTO.Gender,
                phoneNumber = registrationRequestDTO.PhoneNumber
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            user.password = "";

            var token = _tokenService.GenerateToken(user);
            AuthorizationResponseDTO loginResponseDTO = new AuthorizationResponseDTO()
            {
                Token = token,
            };

            return loginResponseDTO;

        }

        public async Task<ProfileResponseDTO> GetProfile(string token)
        {
            string id = _tokenService.GetUserId(token);

            if (!string.IsNullOrEmpty(id))
            {
                var user = await _db.Users.FirstOrDefaultAsync(user => user.Id.ToString() == id);

                if (user != null)
                {
                    ProfileResponseDTO profileResponseDTO = new ProfileResponseDTO()
                    {
                        id = user.Id,
                        birthDate = user.birthDate,
                        gender = user.gender,
                        phoneNumber = user.phoneNumber,
                        fullName = user.fullName,
                        email = user.email,
                        createTime = user.createTime
                    };

                    return profileResponseDTO;
                }
                else throw new NotFoundException("User not found.");
            }

            throw new UnauthorizedAccessException();   
        }

        
    }
}
