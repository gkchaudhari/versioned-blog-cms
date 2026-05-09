namespace CmsBackend.Domain.common
{
    public class BaseEntity
    {

        public bool Archived { get; set; } = false;
        public bool Active { get; set; } = true;
        public int Version { get; set; } = 1;
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    }

    public class PagedResult<T>
    {
        public List<T> Data { get; set; }
        public int TotalCount { get; set; }
        public int page { get; set; }
        public int PageSize { get; set; }
    }
}
