using ConsoleApp1.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp1.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users { set; get; }
        //public DbSet<RevokedToken> RevokedTokens { get; set; }

    }
}
