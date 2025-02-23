using Core.IServices;
using Infrastructure.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController(IUserService _userService) : ControllerBase
    {
        
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto request)
        {
            var result = await _userService.RegisterAsync(request);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            var result = await _userService.LoginAsync(request);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
