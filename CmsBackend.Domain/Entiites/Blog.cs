using CmsBackend.Domain.common;
using CmsBackend.Domain.Enums;

namespace CmsBackend.Domain.Entiites
{
    public class Blog : BaseEntity
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public Guid AuthorId { get; set; }
        public User Author { get; set; } = null!;
        public BlogStatus Status { get; set; } = BlogStatus.Draft;
    }

}
