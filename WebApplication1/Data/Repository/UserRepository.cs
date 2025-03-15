using Microsoft.EntityFrameworkCore;
using WebApplication1.Data.Entities;
using WebApplication1.Exceptions;

namespace WebApplication1.Data.Repository
{
    public class UserRepository
    {
        private readonly AppDbContext _db;
        public UserRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            var user = await _db.Users.FirstOrDefaultAsync(user => user.Email == email);

            if (user == null)
            {
                throw new NotFoundException("Такого пользователя не существует");
            }
            return user;
        }
    }
}
