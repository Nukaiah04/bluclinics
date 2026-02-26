using System.Data.Common;
using BluClinicsApi.Entitys;
using Microsoft.EntityFrameworkCore;

namespace BluClinicsApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<OTPS> OTPS { get; set; }
        public DbSet<IDGenerator> IDGenerator { get; set; }
    }
}

