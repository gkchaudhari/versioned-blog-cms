using CmsBackend.Application.Users.Interfaces;
using CmsBackend.Domain.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CmsBackend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = _userService.GetAllUsers();
            return Ok(users);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("assign-role")]
        public IActionResult AssignRole(AssignRoleDto dto)
        {
            _userService.AssignRole(dto);
            return Ok(new { message = "Role updated successfully" });
        }
    }
}
