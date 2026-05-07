using CmsBackend.Domain.Enums;

namespace CmsBackend.Domain.Entiites
{
    public class Blog
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public Guid AuthorId { get; set; }
        public User Author { get; set; } = null!;
        public BlogStatus Status { get; set; } = BlogStatus.Draft;
        public List<BlogVersion> Versions { get; set; } = new List<BlogVersion>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class BlogVersion
    {
        public int Id { get; set; }
        public int BlogId { get; set; }
        public Blog Blog { get; set; } = null!;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
