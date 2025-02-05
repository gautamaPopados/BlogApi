﻿using Microsoft.EntityFrameworkCore;
using WebApplication1.AuthentificationServices;
using WebApplication1.Data;
using WebApplication1.Data.DTO;
using WebApplication1.Data.Entities;
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
            var user = _db.Users.FirstOrDefault(x => x.Email == email);
            
            if (user == null)
            {
                return true;
            }
            return false;
        }

        public bool IsUniquePhoneNumber(string phoneNumber)
        {
            var user = _db.Users.FirstOrDefault(x => x.PhoneNumber == phoneNumber);
            
            if (user == null)
            {
                return true;
            }
            return false;
        }

        public async Task<TokenResponse> Login(LoginCredentials loginRequestDTO)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == loginRequestDTO.email);

            if (user == null)
            {
                throw new BadRequestException("email or password is incorrect");
            }
            else if (!BCrypt.Net.BCrypt.Verify(loginRequestDTO.password, user.Password))
            {
                throw new BadRequestException("email or password is incorrect");
            }

            
            var token = _tokenService.GenerateToken(user);

            TokenResponse loginResponseDTO = new TokenResponse()
            {
                token = token,
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


        public async Task<TokenResponse> Register(UserRegistrationModel registrationRequestDTO)
        {

            if (!IsUniqueUser(registrationRequestDTO.email))
            {
                throw new BadRequestException($"Username '{registrationRequestDTO.email}' is already taken.");
            }

            User user = new User()
            {
                FullName = registrationRequestDTO.fullName,
                Email = registrationRequestDTO.email,
                Password = BCrypt.Net.BCrypt.HashPassword(registrationRequestDTO.password),
                BirthDate = registrationRequestDTO.birthDate,
                CreateTime = DateTime.UtcNow,
                Gender = registrationRequestDTO.gender,
                PhoneNumber = registrationRequestDTO.phoneNumber
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            user.Password = "";

            var token = _tokenService.GenerateToken(user);

            TokenResponse loginResponseDTO = new TokenResponse()
            {
                token = token,
            };

            return loginResponseDTO;

        }

        public async Task<UserDto> GetProfile(string token)
        {
            string id = _tokenService.GetUserId(token);

            if (!string.IsNullOrEmpty(id))
            {
                var user = await _db.Users.FirstOrDefaultAsync(user => user.Id.ToString() == id);

                if (user != null)
                {
                    UserDto profileResponseDTO = new UserDto()
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
            }

            throw new UnauthorizedAccessException();   
        }

        public async Task Edit(string token, UserEditModel model)
        {
            string id = _tokenService.GetUserId(token);

            if (!IsUniqueUser(model.email))
            {
                throw new BadRequestException($"Username '{model.email}' is already taken.");
            }

            if (!IsUniquePhoneNumber(model.phoneNumber))
            {
                throw new BadRequestException($"Phone Number '{model.phoneNumber}' is already taken.");
            }

            if (!string.IsNullOrEmpty(id))
            {
                var user = await _db.Users.FirstOrDefaultAsync(user => user.Id.ToString() == id);

                if (user != null)
                {
                    user.PhoneNumber = model.phoneNumber;
                    user.FullName = model.fullName;
                    user.BirthDate = model.birthDate;
                    user.Gender = model.gender;
                    user.Email = model.email;

                    await _db.SaveChangesAsync();
                }
                else throw new NotFoundException("User not found.");
            }
            else throw new UnauthorizedAccessException();
        }
    }
}
