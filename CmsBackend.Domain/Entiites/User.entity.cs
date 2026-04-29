using CmsBackend.Domain.common;
using CmsBackend.Domain.Enums;

namespace CmsBackend.Domain.Entiites
{
    public class User : BaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public Roles Role { get; set; } // Admin / User
    }
}
