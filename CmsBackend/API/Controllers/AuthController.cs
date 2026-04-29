using CmsBackend.Application.Auth.Interfaces;
using CmsBackend.Domain.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CmsBackend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth)
        {
            _auth = auth;
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterUser dto)
        {
            try
            {
                var token = _auth.Register(dto);
                return Ok(new { token });
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public IActionResult Login(Login dto)
        {
            try
            {
                var token = _auth.Login(dto);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
          
        }

    }
}
