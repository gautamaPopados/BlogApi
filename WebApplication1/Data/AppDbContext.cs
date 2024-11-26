using Microsoft.EntityFrameworkCore;
using WebApplication1.Data.Entities;

namespace WebApplication1.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users { set; get; }
        public DbSet<Community> Communities { set; get; }

        public DbSet<Tag> Tags { set; get; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Community>()
                .HasMany(c => c.Users)
                .WithMany(s => s.Communities)
                .UsingEntity<CommunityUser>(
                   j => j.HasOne(pt => pt.User).WithMany(t => t.CommunityUsers).HasForeignKey(pt => pt.UserId),
                   j => j.HasOne(pt => pt.Community).WithMany(p => p.CommunityUsers).HasForeignKey(pt => pt.CommunityId),
                   j =>
                   {
                       j.HasKey(t => new { t.Id });
                       j.ToTable("CommunityUser");
                   });
        }
    }
}
