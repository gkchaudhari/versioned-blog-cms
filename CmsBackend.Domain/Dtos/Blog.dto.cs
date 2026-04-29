using CmsBackend.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CmsBackend.Domain.Dtos
{
    public class CreateBlogRequest
    {
        [Required, MaxLength(500)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Excerpt { get; set; }

        public string? CoverImageUrl { get; set; }

        public List<string> Tags { get; set; } = new();

        [MaxLength(200)]
        public string? Slug { get; set; }  // Auto-generated from Title if omitted

        [MaxLength(500)]
        public string? ChangeNote { get; set; }

        // Set this to schedule publish; null = not scheduled
        public DateTime? ScheduledPublishAt { get; set; }
    }

    public class UpdateBlogRequest
    {
        [Required, MaxLength(500)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Excerpt { get; set; }

        public string? CoverImageUrl { get; set; }

        public List<string> Tags { get; set; } = new();

        [MaxLength(500)]
        public string? ChangeNote { get; set; }

        // Update schedule (null clears existing schedule)
        public DateTime? ScheduledPublishAt { get; set; }
    }

    public class PublishBlogRequest
    {
        // Optional: schedule for future publish instead of publishing immediately
        public DateTime? ScheduledAt { get; set; }
    }

    public class RollbackRequest
    {
        [MaxLength(500)]
        public string? ChangeNote { get; set; }
    }

    // ── Responses ─────────────────────────────────────────────────────────────

    public class BlogResponse
    {
        public Guid Id { get; set; }
        public string Slug { get; set; } = string.Empty;
        public Guid AuthorId { get; set; }
        public BlogStatus Status { get; set; }
        public DateTime? ScheduledPublishAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public BlogVersionResponse? CurrentVersion { get; set; }
    }

    public class BlogVersionResponse
    {
        public Guid Id { get; set; }
        public Guid BlogId { get; set; }
        public int VersionNumber { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? Excerpt { get; set; }
        public string? CoverImageUrl { get; set; }
        public List<string> Tags { get; set; } = new();
        public Guid CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ChangeNote { get; set; }
    }

    public class BlogVersionSummary
    {
        public Guid Id { get; set; }
        public int VersionNumber { get; set; }
        public string Title { get; set; } = string.Empty;
        public Guid CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ChangeNote { get; set; }
    }
}
