using Microsoft.EntityFrameworkCore;
using WebApplication1.Data.Entities;

namespace WebApplication1.Data
{
    public class AddressContext : DbContext
    {
        public AddressContext(DbContextOptions<AddressContext> options) : base(options) { }
        public DbSet<AddressElement> AddrElements { get; set; }
        public DbSet<Hierarchy> Hierarchies { get; set; }
        public DbSet<House> Houses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AddressElement>().ToTable("as_addr_obj", "fias");
            modelBuilder.Entity<Hierarchy>().ToTable("as_adm_hierarchy", "fias");
            modelBuilder.Entity<House>().ToTable("as_houses", "fias");
        }
    }
}
