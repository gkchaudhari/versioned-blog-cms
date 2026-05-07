using CmsBackend.Domain.common;
using CmsBackend.Domain.Enums;

namespace CmsBackend.Domain.Entiites
{
    public class User : BaseEntity
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public List<Blog> Blogs { get; set; } = new List<Blog>();
        public Roles Role { get; set; }
    }
}
