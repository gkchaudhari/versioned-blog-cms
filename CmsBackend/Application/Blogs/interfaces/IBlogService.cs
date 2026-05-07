using CmsBackend.Domain.Dtos;
using CmsBackend.Domain.Entiites;

namespace CmsBackend.Application.Blogs.Interfaces
{
    public interface IBlogService
    {
        public Task<List<Blog>> GetAllBlogAsync();
        public Task<BlogResponseDto> CreateBlogAsync(BlogCreateDto dto, Guid userId);
        public Task<BlogResponseDto> GetBlogAsync(Guid blogId);
        public Task<BlogResponseDto> UpdateBlogAsync(Guid blogId, BlogUpdateDto dto);
        public Task<BlogResponseDto> DeleteBlogAsync(Guid blogId);

        public Task<List<Blog>> GetAllBlogByAuthor(Guid AuthorId);

    }
}
