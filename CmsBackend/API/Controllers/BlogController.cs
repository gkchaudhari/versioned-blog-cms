using CmsBackend.Application.Blog.interfaces;
using CmsBackend.Domain.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CmsBackend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly IBlogVersioningService _service;

        public BlogController(IBlogVersioningService service)
        {
            _service = service;
        }

        // POST /api/blogs
        // Create a new blog as a draft (version 1 is created automatically)
        [HttpPost]
        public async Task<IActionResult> CreateDraft([FromBody] CreateBlogRequest request)
        {
            var userId = GetCurrentUserId();
            var blog = await _service.CreateDraftAsync(request, userId);
            return CreatedAtAction(nameof(GetById), new { id = blog.Id }, blog);
        }

        // GET /api/blogs/{id}
        // Get blog with its current (latest active) version
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var blog = await _service.GetByIdAsync(id);
            return blog == null ? NotFound() : Ok(blog);
        }

        // PUT /api/blogs/{id}
        // Save a new version of the blog (always appends; never overwrites history)
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> SaveVersion(Guid id, [FromBody] UpdateBlogRequest request)
        {
            var userId = GetCurrentUserId();
            var blog = await _service.SaveNewVersionAsync(id, request, userId);
            return Ok(blog);
        }

        // GET /api/blogs/{id}/versions
        // List all versions (newest first) — returns summaries, not full content
        [HttpGet("{id:guid}/versions")]
        public async Task<IActionResult> GetVersionHistory(Guid id)
        {
            var history = await _service.GetVersionHistoryAsync(id);
            return Ok(history);
        }

        // GET /api/blogs/{id}/versions/{versionId}
        // Get a specific version's full content (for diff/preview)
        [HttpGet("{id:guid}/versions/{versionId:guid}")]
        public async Task<IActionResult> GetVersion(Guid id, Guid versionId)
        {
            var version = await _service.GetVersionAsync(id, versionId);
            return version == null ? NotFound() : Ok(version);
        }

        // POST /api/blogs/{id}/publish
        // Publish immediately, or schedule for a future datetime
        [HttpPost("{id:guid}/publish")]
        public async Task<IActionResult> Publish(Guid id, [FromBody] PublishBlogRequest request)
        {
            var userId = GetCurrentUserId();
            var blog = await _service.PublishAsync(id, request, userId);
            return Ok(blog);
        }

        // POST /api/blogs/{id}/rollback/{versionId}
        // Rollback: copies the target version as a new draft version
        [HttpPost("{id:guid}/rollback/{versionId:guid}")]
        public async Task<IActionResult> Rollback(Guid id, Guid versionId, [FromBody] RollbackRequest request)
        {
            var userId = GetCurrentUserId();
            var blog = await _service.RollbackToVersionAsync(id, versionId, request, userId);
            return Ok(blog);
        }

        // POST /api/blogs/{id}/archive
        // Archive the blog (removes from public; history preserved)
        [HttpPost("{id:guid}/archive")]
        public async Task<IActionResult> Archive(Guid id)
        {
            var userId = GetCurrentUserId();
            var blog = await _service.ArchiveAsync(id, userId);
            return Ok(blog);
        }

        // ── Helpers ────────────────────────────────────────────────────────────

        private Guid GetCurrentUserId()
        {
            var raw = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub")
                ?? throw new UnauthorizedAccessException("User identity not found in token.");

            return Guid.Parse(raw);
        }
    }
}
