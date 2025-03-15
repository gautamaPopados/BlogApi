using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Crypto;
using WebApplication1.Data;
using WebApplication1.Data.BannedToken;
using WebApplication1.Data.DTO.Auth;
using WebApplication1.Data.DTO.User;
using WebApplication1.Data.Entities;
using WebApplication1.Exceptions;
using WebApplication1.Services.IServices;

namespace WebApplication1.Services
{
    public class UserService : IUserService
    {
        private readonly ITokenService _tokenService;
        private readonly UserManager<User> _userManager;
        private readonly RedisRepository _redisRepository;
        private readonly SignInManager<User> _signInManager;

        public UserService(
            ITokenService tokenService,
            UserManager<User> userManager,
            RedisRepository redisRepository,
            SignInManager<User> signInManager
        )
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _redisRepository = redisRepository;
            _signInManager = signInManager;
        }

        public async Task<bool> IsUniqueUser(string email)
        {
            var user = await _userManager.FindByEmailAsync( email );

            if (user == null)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> IsUniquePhoneNumber(string phoneNumber)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);


            if (user == null)
            {
                return true;
            }
            return false;
        }

        public async Task<AuthResponseDTO> Login(LoginRequestDTO loginData)
        {
            var user = await _userManager.FindByEmailAsync(loginData.Email);

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginData.Password, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var accessToken = await _tokenService.GenerateAccessToken(user.Id);
                var refreshToken = await _tokenService.GenerateRefreshToken(user.Id);

                await _tokenService.SaveRefreshTokenAsync(refreshToken.Token, user.Id);

                var tokenPair = new AuthResponseDTO
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken.Token,
                };
                
                return tokenPair;
            }
            else
            {
                throw new BadRequestException("Неверный логин или пароль");
            }
        }

        public async Task Logout(string accessToken)
        {
            var userId = _tokenService.GetUserIdFromToken(accessToken);

            var user = await _userManager.Users.FirstOrDefaultAsync(user => user.Id == userId);
            if (user == null)
            {
                throw new NotFoundException("Такого пользователя не существует");
            }

            user.RefreshToken = null;
            await _userManager.UpdateAsync(user);
            await _redisRepository.AddTokenBlackList(accessToken);
        }

        public async Task<AuthResponseDTO> Register(RegistrationRequestDTO registrationData)
        {
            User newUser = new User()
            {
                FullName = registrationData.fullName,
                Email = registrationData.email,
                BirthDate = registrationData.birthDate,
                CreateTime = DateTime.UtcNow,
                Gender = registrationData.gender,
                PhoneNumber = registrationData.phoneNumber,
            };
            newUser.UserName = newUser.Email;
            newUser.SecurityStamp = Guid.NewGuid().ToString();

            var creatingResult = await _userManager.CreateAsync(newUser, registrationData.password);

            if (creatingResult.Succeeded)
            {
                var loginData = new LoginRequestDTO
                {
                    Email = registrationData.email,
                    Password = registrationData.password,
                };

                var tokenInfo = await Login(loginData);

                return tokenInfo;
            }
            else
            {
                
                var errors = creatingResult.Errors.Select(error => error.Description);
                var errorMessage = string.Join(", ", errors);
                throw new BadRequestException($"Ошибка при регистрации: {errorMessage}");
            }
        }


        public async Task<ProfileResponseDTO> GetProfile(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user != null)
            {
                ProfileResponseDTO profileResponseDTO = new ProfileResponseDTO()
                {
                    id = user.Id,
                    birthDate = user.BirthDate,
                    gender = user.Gender,
                    phoneNumber = user.PhoneNumber,
                    fullName = user.FullName,
                    email = user.Email,
                    createTime = user.CreateTime
                };

                return profileResponseDTO;
            }
            else throw new NotFoundException("User not found.");
            

            throw new UnauthorizedAccessException();   
        }

        public async Task ChangeProfileInfo(Guid userId, ChangeProfileRequestDTO newProfileInfo)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                throw new NotFoundException("Такого пользователя не существует");

            if (! await IsUniqueUser(newProfileInfo.email))
            {
                throw new BadRequestException($"Username '{newProfileInfo.email}' is already taken.");
            }

            if (! await IsUniquePhoneNumber(newProfileInfo.phoneNumber))
            {
                throw new BadRequestException($"Phone Number '{newProfileInfo.phoneNumber}' is already taken.");
            }

            user.PhoneNumber = newProfileInfo.phoneNumber;
            user.FullName = newProfileInfo.fullName;
            user.BirthDate = newProfileInfo.birthDate;
            user.Gender = newProfileInfo.gender;
            user.Email = newProfileInfo.email;

            //await SyncProfileInfo(userId, newProfileInfo.fullName, newProfileInfo.email);

            user.SecurityStamp = Guid.NewGuid().ToString();

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return;
            }
            else
            {
                throw new BadRequestException($"{result}");
            }
        }

    }
}
