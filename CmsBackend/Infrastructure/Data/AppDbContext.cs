using CmsBackend.Domain.Entiites;
using Microsoft.EntityFrameworkCore;

namespace CmsBackend.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<BlogVersion> BlogVersions { get; set; }
    }
}
