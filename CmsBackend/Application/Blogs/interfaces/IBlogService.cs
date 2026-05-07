using CmsBackend.Domain.Dtos;
using CmsBackend.Domain.Entiites;

namespace CmsBackend.Application.Blogs.Interfaces
{
    public interface IBlogService
    {
        public Task<Blog> CreateBlogAsync(BlogCreateDto dto, Guid userId);
        public Task<BlogResponseDto> GetBlogAsync(int blogId);
    }
}
