using CmsBackend.Domain.Entiites;
using Microsoft.EntityFrameworkCore;

namespace CmsBackend.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        public DbSet<BlogEntity> Blogs => Set<BlogEntity>();
        public DbSet<BlogVersion> BlogVersions => Set<BlogVersion>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── Blog ──────────────────────────────────────────────────
            modelBuilder.Entity<BlogEntity>(b =>
            {
                //Primary Key & Unique Index
                b.HasKey(x => x.Id);
                b.HasIndex(x => x.Slug).IsUnique();

                b.Property(x => x.Status)
                    .HasConversion<string>()
                    .HasMaxLength(20);

                // One-to-many: Blog → BlogVersions
                b.HasMany(x => x.Versions)
                    .WithOne(v => v.Blog)
                    .HasForeignKey(v => v.BlogId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Soft FK to CurrentVersion (no cascade — version list handles its own cascade)
                b.HasOne(x => x.CurrentVersion)
                    .WithMany()
                    .HasForeignKey(x => x.CurrentVersionId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // ── BlogVersion ───────────────────────────────────────────
            modelBuilder.Entity<BlogVersion>(v =>
            {
                v.HasKey(x => x.Id);

                // Composite unique index: (BlogId, VersionNumber)
                v.HasIndex(x => new { x.BlogId, x.VersionNumber }).IsUnique();

                v.Property(x => x.Title).HasMaxLength(500).IsRequired();
                v.Property(x => x.Excerpt).HasMaxLength(1000);
                v.Property(x => x.ChangeNote).HasMaxLength(500);

                // Store Tags as a JSON column (PostgreSQL jsonb)
                v.Property(x => x.Tags)
                    .HasColumnType("jsonb")
                    .HasConversion(
                        tags => System.Text.Json.JsonSerializer.Serialize(tags, (System.Text.Json.JsonSerializerOptions?)null),
                        json => System.Text.Json.JsonSerializer.Deserialize<List<string>>(json, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>()
                    );
            });
        }
    }
}
