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
        public async Task<BlogResponseDto> CreateBlogAsync(BlogCreateDto dto, Guid userId)
        {
            var userExists = await _context.Users
                .AnyAsync(u => u.Id == userId);

            if (!userExists)
            {
                throw new Exception("User not found");
            }

            Blog blog = new Blog
            {
                Id = new Guid(),
                Title = dto.Title,
                Content = dto.Content,
                AuthorId = userId
            };

            _context.Blogs.Add(blog);

            await _context.SaveChangesAsync();

            return blog.Adapt<BlogResponseDto>();
        }

        public async Task<BlogResponseDto> DeleteBlogAsync(Guid blogId)
        {
            var blog = _context.Blogs.FirstOrDefault(b => b.Id == blogId);
            if (blog == null)
            {
                throw new Exception("Invlaid Blog Id");
            }
            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();
            var blogRes = blog.Adapt<BlogResponseDto>();
            return blogRes;
        }

        public async Task<List<Blog>> GetAllBlogAsync()
        {
            var blogs = await _context.Blogs.ToListAsync();
            if (blogs == null || blogs.Count == 0)
            {
                throw new Exception("No Blogs Found");
            }
            return blogs;
        }

        public async Task<List<Blog>> GetAllBlogByAuthor(Guid AuthorId)
        {
            var blogs = await _context.Blogs.Where(b => b.AuthorId == AuthorId).ToListAsync();
            if (blogs == null || blogs.Count == 0)
            {
                throw new Exception("No Blogs Found");
            }
            return blogs;
        }

        public async Task<BlogResponseDto> GetBlogAsync(Guid blogId)
        {
            var blog = await _context.Blogs
                .Include(b => b.Author)
                .FirstOrDefaultAsync(b => b.Id == blogId);

            if (blog == null)
            {
                throw new Exception("Invalid Blog Id");
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

        public async Task<BlogResponseDto> UpdateBlogAsync(Guid blogId, BlogUpdateDto dto)
        {
            var blog = _context.Blogs.FirstOrDefault(b => b.Id == blogId);

            if (blog == null)
            {
                throw new Exception("Blog not found");
            }

            blog.Title = dto.Title;
            blog.Status = dto.Status;
            blog.Content = dto.Content;

            _context.Blogs.Update(blog);
            await _context.SaveChangesAsync();

            var blogRes = blog.Adapt<BlogResponseDto>();
            return blogRes;
        }
    }
}
