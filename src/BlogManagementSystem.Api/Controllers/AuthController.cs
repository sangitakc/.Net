using BlogManagementSystem.Api.DTOs;
using BlogManagementSystem.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogManagementSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService) => _authService = authService;

        [HttpPost("register"), AllowAnonymous]
        public async Task<ActionResult<BaseResponse<string>>> Register(RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            return result.Success
                ? Ok(result)
                : BadRequest(result);
        }

        [HttpPost("login"), AllowAnonymous]
        public async Task<ActionResult<BaseResponse<string>>> Login(LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            return result.Success
                ? Ok(result)
                : Unauthorized(result);
        }
    }
}
