using Microsoft.EntityFrameworkCore;
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
using WebApplication1.Exceptions;
using WebApplication1.Services.IServices;

namespace WebApplication1.Services
{
    public class CommunityService : ICommunityService
    {
        private readonly AppDbContext _db;

        public CommunityService(AppDbContext db, IConfiguration configuration)
        {
            _db = db;
        }

        public async Task<List<CommunityDto>> GetCommunities()
        {
            var communityList = await _db.Communities.ToListAsync();
            var communityDtos = new List<CommunityDto>();

            foreach (var community in communityList)
            {
                var newDto = new CommunityDto()
                {
                    Id = community.Id,
                    Name = community.Name,
                    CreateTime = community.CreateTime,
                    IsClosed = community.IsClosed,
                    Description = community.Description,
                    SubscribersCount = community.SubscribersCount
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
            else { throw new NotFoundException($"Community with id={id} not found in  database"); }

        }
    }
}
