using CmsBackend.Application.Blogs.Interfaces;
using CmsBackend.Domain.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CmsBackend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly IBlogService _service;

        public BlogController(IBlogService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateBlog([FromBody] BlogCreateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var userGuid = Guid.Parse(userId);
            var blog = await _service.CreateBlogAsync(dto, userGuid);

            return CreatedAtAction(nameof(GetBlog), new { id = blog.Id }, blog);
        }

        [HttpGet]
        public async Task<IActionResult> GetBlog(int id)
        {
            var blog = await _service.GetBlogAsync(id);
            if (blog == null)
                return NotFound();
            return Ok(blog);
        }
    }
}
