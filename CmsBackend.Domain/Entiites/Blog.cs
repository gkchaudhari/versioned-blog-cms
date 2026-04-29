using CmsBackend.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CmsBackend.Domain.Entiites
{
    public class BlogEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Slug { get; set; } = string.Empty;
        public Guid AuthorId { get; set; }

        // Points to the currently active version
        public Guid? CurrentVersionId { get; set; }
        public BlogVersion? CurrentVersion { get; set; }

        public BlogStatus Status { get; set; } = BlogStatus.Draft;

        // Null = not scheduled; set to a future UTC datetime to auto-publish
        public DateTime? ScheduledPublishAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation: all versions ever saved for this blog
        public ICollection<BlogVersion> Versions { get; set; } = new List<BlogVersion>();
    }

    public class BlogVersion
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid BlogId { get; set; }
        public BlogEntity Blog { get; set; } = null!;

        public int VersionNumber { get; set; }  // Auto-incremented per blog

        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;  // Markdown / HTML
        public string? Excerpt { get; set; }
        public string? CoverImageUrl { get; set; }
        public List<string> Tags { get; set; } = new();

        public Guid CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Optional note explaining what changed in this version
        public string? ChangeNote { get; set; }
    }

}
