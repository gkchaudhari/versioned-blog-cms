using CmsBackend.Application.Blog.interfaces;
using CmsBackend.Domain.Dtos;
using CmsBackend.Domain.Entiites;
using CmsBackend.Domain.Enums;
using CmsBackend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace CmsBackend.Application.Blog.Services
{
    public class BlogVersioningService : IBlogVersioningService
    {
        private readonly AppDbContext _db;

        public BlogVersioningService(AppDbContext db)
        {
            _db = db;
        }

        // ── Create ─────────────────────────────────────────────────────────────

        public async Task<BlogResponse> CreateDraftAsync(CreateBlogRequest request, Guid userId)
        {
            var slug = string.IsNullOrWhiteSpace(request.Slug)
                ? await GenerateUniqueSlugAsync(request.Title)
                : request.Slug.ToLowerInvariant().Trim();

            if (await _db.Blogs.AnyAsync(b => b.Slug == slug))
                throw new InvalidOperationException($"Slug '{slug}' is already taken.");

            var blog = new BlogEntity
            {
                Slug = slug,
                AuthorId = userId,
                Status = BlogStatus.Draft,
                ScheduledPublishAt = request.ScheduledPublishAt?.ToUniversalTime(),
                Versions = new List<BlogVersion>() // ✅ important
            };

            var version = new BlogVersion
            {
                VersionNumber = 1,
                Title = request.Title,
                Content = request.Content,
                Excerpt = request.Excerpt,
                CoverImageUrl = request.CoverImageUrl,
                Tags = request.Tags,
                CreatedByUserId = userId,
                ChangeNote = request.ChangeNote ?? "Initial draft",
            };

            blog.Versions.Add(version); // ✅ EF will handle BlogId

            _db.Blogs.Add(blog);
            await _db.SaveChangesAsync();

            // ✅ NOW IDs exist
            blog.CurrentVersionId = version.Id;
            await _db.SaveChangesAsync();

            return await GetByIdAsync(blog.Id)
                ?? throw new Exception("Failed to retrieve created blog.");
        }
        // ── Save New Version ───────────────────────────────────────────────────

        public async Task<BlogResponse> SaveNewVersionAsync(Guid blogId, UpdateBlogRequest request, Guid userId)
        {
            var blog = await LoadBlogAsync(blogId);

            if (blog.Status == BlogStatus.Archived)
                throw new InvalidOperationException("Cannot edit an archived blog.");

            var nextVersionNumber = await _db.BlogVersions
                .Where(v => v.BlogId == blogId)
                .MaxAsync(v => (int?)v.VersionNumber) ?? 0;
            nextVersionNumber++;

            var version = new BlogVersion
            {
                BlogId = blog.Id,
                VersionNumber = nextVersionNumber,
                Title = request.Title,
                Content = request.Content,
                Excerpt = request.Excerpt,
                CoverImageUrl = request.CoverImageUrl,
                Tags = request.Tags,
                CreatedByUserId = userId,
                ChangeNote = request.ChangeNote,
            };

            _db.BlogVersions.Add(version);

            blog.CurrentVersionId = version.Id;
            blog.UpdatedAt = DateTime.UtcNow;

            // Update schedule if provided
            if (request.ScheduledPublishAt.HasValue)
                blog.ScheduledPublishAt = request.ScheduledPublishAt.Value.ToUniversalTime();
            else
                blog.ScheduledPublishAt = null;

            await _db.SaveChangesAsync();

            return await GetByIdAsync(blog.Id) ?? throw new Exception("Failed to retrieve updated blog.");
        }

        // ── Publish ────────────────────────────────────────────────────────────

        public async Task<BlogResponse> PublishAsync(Guid blogId, PublishBlogRequest request, Guid userId)
        {
            var blog = await LoadBlogAsync(blogId);

            if (blog.Status == BlogStatus.Archived)
                throw new InvalidOperationException("Cannot publish an archived blog. Unarchive it first.");

            if (blog.CurrentVersionId == null)
                throw new InvalidOperationException("Blog has no content version to publish.");

            if (request.ScheduledAt.HasValue)
            {
                // Schedule for future publish
                blog.ScheduledPublishAt = request.ScheduledAt.Value.ToUniversalTime();
                // Keep as Draft until the background job fires
            }
            else
            {
                // Publish immediately
                blog.Status = BlogStatus.Published;
                blog.ScheduledPublishAt = null;
            }

            blog.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return await GetByIdAsync(blog.Id) ?? throw new Exception("Failed to retrieve published blog.");
        }

        // ── Rollback ───────────────────────────────────────────────────────────

        public async Task<BlogResponse> RollbackToVersionAsync(Guid blogId, Guid versionId, RollbackRequest request, Guid userId)
        {
            var blog = await LoadBlogAsync(blogId);

            if (blog.Status == BlogStatus.Archived)
                throw new InvalidOperationException("Cannot rollback an archived blog.");

            var targetVersion = await _db.BlogVersions
                .FirstOrDefaultAsync<BlogVersion>(v => v.Id == versionId && v.BlogId == blogId)
                ?? throw new KeyNotFoundException($"Version {versionId} not found for blog {blogId}.");

            // Create a new version that is a copy of the target — we never mutate history
            var nextVersionNumber = await _db.BlogVersions
                .Where(v => v.BlogId == blogId)
                .MaxAsync(v => (int?)v.VersionNumber) ?? 0;
            nextVersionNumber++;

            var rollbackVersion = new BlogVersion
            {
                BlogId = blog.Id,
                VersionNumber = nextVersionNumber,
                Title = targetVersion.Title,
                Content = targetVersion.Content,
                Excerpt = targetVersion.Excerpt,
                CoverImageUrl = targetVersion.CoverImageUrl,
                Tags = new List<string>(targetVersion.Tags),
                CreatedByUserId = userId,
                ChangeNote = request.ChangeNote
                    ?? $"Rolled back to v{targetVersion.VersionNumber}",
            };

            _db.BlogVersions.Add(rollbackVersion);

            blog.CurrentVersionId = rollbackVersion.Id;
            blog.Status = BlogStatus.Draft;  // Rollback always goes back to Draft
            blog.ScheduledPublishAt = null;
            blog.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return await GetByIdAsync(blog.Id) ?? throw new Exception("Failed to retrieve blog after rollback.");
        }

        // ── Archive ────────────────────────────────────────────────────────────

        public async Task<BlogResponse> ArchiveAsync(Guid blogId, Guid userId)
        {
            var blog = await LoadBlogAsync(blogId);

            blog.Status = BlogStatus.Archived;
            blog.ScheduledPublishAt = null;
            blog.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return await GetByIdAsync(blog.Id) ?? throw new Exception("Failed to retrieve archived blog.");
        }

        // ── Queries ────────────────────────────────────────────────────────────

        public async Task<BlogResponse?> GetByIdAsync(Guid blogId)
        {
            var blog = await _db.Blogs
                .Include(b => b.CurrentVersion)
                .FirstOrDefaultAsync(b => b.Id == blogId);

            return blog == null ? null : MapToResponse(blog);
        }

        public async Task<List<BlogVersionSummary>> GetVersionHistoryAsync(Guid blogId)
        {
            return await _db.BlogVersions
                .Where(v => v.BlogId == blogId)
                .OrderByDescending(v => v.VersionNumber)
                .Select(v => new BlogVersionSummary
                {
                    Id = v.Id,
                    VersionNumber = v.VersionNumber,
                    Title = v.Title,
                    CreatedByUserId = v.CreatedByUserId,
                    CreatedAt = v.CreatedAt,
                    ChangeNote = v.ChangeNote,
                })
                .ToListAsync();
        }

        public async Task<BlogVersionResponse?> GetVersionAsync(Guid blogId, Guid versionId)
        {
            var v = await _db.BlogVersions
                .FirstOrDefaultAsync(x => x.Id == versionId && x.BlogId == blogId);

            return v == null ? null : MapVersionToResponse(v);
        }

        // ── Background Job: Process Scheduled Publishes ────────────────────────

        public async Task ProcessScheduledPublishesAsync()
        {
            var due = await _db.Blogs
                .Where(b => b.Status == BlogStatus.Draft
                         && b.ScheduledPublishAt.HasValue
                         && b.ScheduledPublishAt.Value <= DateTime.UtcNow)
                .ToListAsync();

            foreach (var blog in due)
            {
                blog.Status = BlogStatus.Published;
                blog.ScheduledPublishAt = null;
                blog.UpdatedAt = DateTime.UtcNow;
            }

            if (due.Count > 0)
                await _db.SaveChangesAsync();
        }

        // ── Helpers ────────────────────────────────────────────────────────────

        private async Task<BlogEntity> LoadBlogAsync(Guid blogId)
        {
            return await _db.Blogs.FirstOrDefaultAsync(b => b.Id == blogId)
                ?? throw new KeyNotFoundException($"Blog {blogId} not found.");
        }

        private async Task<string> GenerateUniqueSlugAsync(string title)
        {
            var baseSlug = Regex.Replace(title.ToLowerInvariant(), @"[^a-z0-9\s-]", "")
                .Trim().Replace(" ", "-");
            baseSlug = Regex.Replace(baseSlug, @"-+", "-").Trim('-');

            var slug = baseSlug;
            var counter = 1;

            while (await _db.Blogs.AnyAsync(b => b.Slug == slug))
                slug = $"{baseSlug}-{counter++}";

            return slug;
        }

        private static BlogResponse MapToResponse(BlogEntity b) => new()
        {
            Id = b.Id,
            Slug = b.Slug,
            AuthorId = b.AuthorId,
            Status = b.Status,
            ScheduledPublishAt = b.ScheduledPublishAt,
            CreatedAt = b.CreatedAt,
            UpdatedAt = b.UpdatedAt,
            CurrentVersion = b.CurrentVersion == null ? null : MapVersionToResponse(b.CurrentVersion),
        };

        private static BlogVersionResponse MapVersionToResponse(BlogVersion v) => new()
        {
            Id = v.Id,
            BlogId = v.BlogId,
            VersionNumber = v.VersionNumber,
            Title = v.Title,
            Content = v.Content,
            Excerpt = v.Excerpt,
            CoverImageUrl = v.CoverImageUrl,
            Tags = v.Tags,
            CreatedByUserId = v.CreatedByUserId,
            CreatedAt = v.CreatedAt,
            ChangeNote = v.ChangeNote,
        };
    }
}
