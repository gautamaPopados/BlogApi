using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data.DTO;
using WebApplication1.Data.Entities;

namespace WebApplication1.Data
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid, IdentityUserClaim<Guid>,
    IdentityUserRole<Guid>, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users { set; get; } = null!;
        public DbSet<Community> Communities { set; get; }
        public DbSet<Comment> Comments { set; get; }
        public DbSet<Post> Posts { set; get; }
        public DbSet<EmailQueue> EmailQueue { set; get; }
        public DbSet<Tag> Tags { set; get; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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

            modelBuilder
                .Entity<Post>()
                .HasMany(c => c.Users)
                .WithMany(s => s.Posts)
                .UsingEntity<PostUserLike>(
                   j => j.HasOne(pt => pt.User).WithMany(t => t.UserLikes).HasForeignKey(pt => pt.UserId),
                   j => j.HasOne(pt => pt.Post).WithMany(p => p.UserLikes).HasForeignKey(pt => pt.PostId),
                   j =>
                   {
                       j.HasKey(t => new { t.Id });
                       j.ToTable("UserLike");
                   });

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId);
        }
    }
}
