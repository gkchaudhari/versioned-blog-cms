using CmsBackend.Domain.Dtos;

namespace CmsBackend.Application.Blog.interfaces
{
    public interface IBlogVersioningService
    {
        Task<BlogResponse> CreateDraftAsync(CreateBlogRequest request, Guid userId);
        Task<BlogResponse> SaveNewVersionAsync(Guid blogId, UpdateBlogRequest request, Guid userId);
        Task<BlogResponse> PublishAsync(Guid blogId, PublishBlogRequest request, Guid userId);
        Task<BlogResponse> RollbackToVersionAsync(Guid blogId, Guid versionId, RollbackRequest request, Guid userId);
        Task<BlogResponse> ArchiveAsync(Guid blogId, Guid userId);
        Task<BlogResponse?> GetByIdAsync(Guid blogId);
        Task<List<BlogVersionSummary>> GetVersionHistoryAsync(Guid blogId);
        Task<BlogVersionResponse?> GetVersionAsync(Guid blogId, Guid versionId);
        Task ProcessScheduledPublishesAsync();  // Called by background job
    }
}
