using CmsBackend.Domain.common;
using CmsBackend.Domain.Enums;

namespace CmsBackend.Domain.Entiites
{
    public class Blog : BaseEntity
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? CoverImageUrl { get; set; }
        public string? Slug { get; set; }
        public string? Category { get; set; }
        public Guid AuthorId { get; set; }
        public User Author { get; set; } = null!;
        public DateTime? PublishedAt { get; set; }
        public BlogStatus Status { get; set; } = BlogStatus.Draft;
    }

}
