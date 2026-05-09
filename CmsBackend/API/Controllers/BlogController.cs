using CmsBackend.Application.Blogs.Interfaces;
using CmsBackend.Domain.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CmsBackend.API.Controllers
{
    [Route("api/[controller]/[action]")]
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
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                    return Unauthorized();

                var userGuid = Guid.Parse(userId);
                var blog = await _service.CreateBlogAsync(dto, userGuid);


                return CreatedAtAction(nameof(GetBlogById), new { id = blog.Id }, blog);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetBlogById(Guid id)
        {
            try
            {
                var blog = await _service.GetBlogAsync(id);
                if (blog == null)
                    return NotFound();
                return Ok(blog);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPut]
        public async Task<IActionResult> UpdateBlog(Guid id, [FromBody] BlogUpdateDto dto)
        {
            try
            {
                var res = await _service.UpdateBlogAsync(id, dto);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteBlog(Guid id)
        {
            try
            {
                var res = await _service.DeleteBlogAsync(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBlogs([FromQuery] int page, [FromQuery] int pageSize)
        {
            try
            {
                var res = await _service.GetAllBlogAsync(page, pageSize);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllBlogsByAuthor()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userGuid = Guid.Parse(userId);
                var res = await _service.GetAllBlogByAuthor(userGuid);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }



    }
}
