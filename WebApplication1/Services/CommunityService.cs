﻿using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;
using WebApplication1.AuthentificationServices;
using WebApplication1.Data;
using WebApplication1.Data.DTO;
using WebApplication1.Data.Entities;
using WebApplication1.Data.Enums;
using WebApplication1.Exceptions;
using WebApplication1.Services.IServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApplication1.Services
{
    public class CommunityService : ICommunityService
    {
        private readonly AppDbContext _db;
        private readonly TokenService _tokenService;

        public CommunityService(AppDbContext db, IConfiguration configuration, TokenService tokenService)
        {
            _db = db;
            _tokenService = tokenService;
        }

        public async Task<List<CommunityDto>> GetCommunities()
        {
            var communityList = await _db.Communities.ToListAsync();
            var communityDtos = new List<CommunityDto>();

            foreach (var community in communityList)
            {
                var newDto = new CommunityDto()
                {
                    id = community.Id,
                    name = community.Name,
                    createTime = community.CreateTime,
                    isClosed = community.IsClosed,
                    description = community.Description,
                    subscribersCount = community.SubscribersCount
                };

                communityDtos.Add(newDto);
            }

            return communityDtos;
        }

        public async Task<CommunityFullDto> GetCommunityById(Guid id)
        {
            var community = await _db.Communities.Include(c => c.CommunityUsers).ThenInclude(c => c.User).FirstOrDefaultAsync(c => c.Id == id);

            if (community != null)
            {
                var admins = community.CommunityUsers.Where(t => t.Role == Data.Enums.CommunityRole.Administrator).Select(t => t.User);

                var adminsDtos = new List<UserDto>();

                foreach (var admin in admins)
                {
                    var newDto = new UserDto()
                    {
                        id = admin.Id,
                        fullName = admin.FullName,
                        email = admin.Email,
                        phoneNumber = admin.PhoneNumber,
                        birthDate = admin.BirthDate,
                        gender = admin.Gender,
                        createTime = admin.CreateTime
                    };

                    adminsDtos.Add(newDto);
                }

                return new CommunityFullDto()
                {
                    id = id,
                    createTime = DateTime.Now,
                    name = community.Name,
                    description = community.Description,
                    isClosed = community.IsClosed,
                    subscribersCount = community.SubscribersCount,
                    administrators = adminsDtos
                };
            }
            else throw new NotFoundException($"Community with id={id} not found in  database");

        }

        public async Task SubscribeUser(string token, Guid id)
        {
            string userId = _tokenService.GetUserId(token);
            var user = await _db.Users.Include(user => user.CommunityUsers).FirstOrDefaultAsync(user => user.Id.ToString() == userId);

            if (user == null)
            {
                throw new UnauthorizedAccessException($"User with id={userId} not found in database.");
            }

            var community = await _db.Communities.FirstOrDefaultAsync(c => c.Id == id);

            if (community == null)
            {
                throw new NotFoundException($"Community with id={id} not found in  database.");
            }

            if (user.CommunityUsers.Exists(cu => cu.UserId == user.Id && cu.CommunityId == id))
            {
                throw new BadRequestException($"User with id={userId} already subscribed to the community with id={id}");
            }

            community.CommunityUsers.Add(
                new CommunityUser()
                {
                    User = user,
                    Community = community,
                    Role = Data.Enums.CommunityRole.Subscriber
                });
            community.SubscribersCount++;

            await _db.SaveChangesAsync();

            return;
        }
        public async Task UnsubscribeUser(string token, Guid id)
        {
            string userId = _tokenService.GetUserId(token);
            var user = await _db.Users.Include(user => user.CommunityUsers).FirstOrDefaultAsync(user => user.Id.ToString() == userId);

            if (user == null)
            {
                throw new UnauthorizedAccessException($"User with id={userId} not found in database.");
            }

            var community = await _db.Communities.FirstOrDefaultAsync(c => c.Id == id);

            if (community == null)
            {
                throw new NotFoundException($"Community with id={id} not found in database.");
            }

            var communityUser = user.CommunityUsers.FirstOrDefault(cu => cu.UserId == user.Id && cu.CommunityId == id && cu.Role == Data.Enums.CommunityRole.Subscriber);

            if (communityUser == null)
            {
                throw new BadRequestException($"User with id={userId} is not subscribed to the community with id={id}");
            }

            community.CommunityUsers.Remove(communityUser);
            community.SubscribersCount--;

            await _db.SaveChangesAsync();

            return;
        }
        public async Task<List<CommunityUserDto>> GetUserCommunities(string token)
        {
            string userId = _tokenService.GetUserId(token);

            var user = await _db.Users.Include(user => user.CommunityUsers).FirstOrDefaultAsync(user => user.Id.ToString() == userId);

            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }

            var cuList = user.CommunityUsers
                .GroupBy(cu => cu.CommunityId)
                .Select(g => g.OrderByDescending(cu => cu.Role).FirstOrDefault()) 
                .Select(cu => new CommunityUserDto
                {
                    userId = cu.UserId,
                    communityId = cu.CommunityId,
                    role = cu.Role
                })
                .ToList();

            return cuList;
        }

        public async Task<CommunityRole?> GetGreatestRole(string token, Guid id)
        {
            string userId = _tokenService.GetUserId(token);
            var user = await _db.Users.Include(user => user.CommunityUsers).FirstOrDefaultAsync(user => user.Id.ToString() == userId);

            if (user == null)
            {
                throw new UnauthorizedAccessException($"User with id={userId} not found in database.");
            }

            var community = await _db.Communities.FirstOrDefaultAsync(c => c.Id == id);

            if (community == null)
            {
                throw new NotFoundException($"Community with id={id} not found in database.");
            }

            var communityUser = user.CommunityUsers
                .GroupBy(cu => cu.CommunityId)
                .Select(g => g.OrderByDescending(cu => cu.Role)
                               .FirstOrDefault()).FirstOrDefault();

            if (communityUser == null)
            {
                return null;
            }
            else
            {
                return communityUser.Role;
            }
        }
    }
}
