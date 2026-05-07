using CmsBackend.Application.Blogs.Interfaces;
using CmsBackend.Domain.Dtos;
using CmsBackend.Domain.Entiites;
using CmsBackend.Infrastructure.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace CmsBackend.Application.Blogs.Services
{
    public class BlogService : IBlogService
    {
        private readonly AppDbContext _context;

        public BlogService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Blog> CreateBlogAsync(BlogCreateDto dto, Guid userId)
        {
            Blog blog = new Blog
            {
                Title = dto.Title,
                Content = dto.Content,
                AuthorId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Blogs.Add(blog);
            await _context.SaveChangesAsync();
            return blog;
        }

        public async Task<BlogResponseDto> GetBlogAsync(int blogId)
        {
            var blog = await _context.Blogs
                .Include(b => b.Author)
                .FirstOrDefaultAsync(b => b.Id == blogId);

            if (blog == null)
            {
                return null;
            }

            //Mapster make it simple easy.
            //BlogResponseDto blogRes = new BlogResponseDto
            //{
            //    Id = blog.Id,
            //    Title = blog.Title,
            //    Content = blog.Content,
            //    AuthorName = blog.Author.Name,
            //    Status = blog.Status.ToString()
            //};

            var blogRes = blog.Adapt<BlogResponseDto>();

            return blogRes;
        }
    }
}
