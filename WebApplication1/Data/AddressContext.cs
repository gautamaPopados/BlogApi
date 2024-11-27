using Microsoft.EntityFrameworkCore;
using WebApplication1.Data.Entities;

namespace WebApplication1.Data
{
    public class AddressContext : DbContext
    {
        public AddressContext(DbContextOptions<AddressContext> options) : base(options) { }
        public DbSet<AddrObj> AddrObjs { get; set; }
        public DbSet<AdmHierarchy> AdmHierarchies { get; set; }
        public DbSet<House> Houses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AddrObj>().ToTable("as_addr_obj");
            modelBuilder.Entity<AdmHierarchy>().ToTable("as_adm_hierarchy");
            modelBuilder.Entity<House>().ToTable("as_houses");
        }
    }
}
