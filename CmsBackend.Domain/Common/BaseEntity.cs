namespace CmsBackend.Domain.common
{
    public class BaseEntity
    {

        public bool Archived { get; set; } = false;
        public bool Active { get; set; } = true;
        public int Version { get; set; } = 1;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
