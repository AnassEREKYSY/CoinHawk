using Infrastructure.Dtos;
using Infrastructure.IServices;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController(IUserService _userService, IJwtTokenDecoderService _jwtTokenDecoderService) : ControllerBase
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

        [HttpPost("login-callBack")]
        public async Task<IActionResult> LoginCallBack([FromBody] LoginDto request)
        {
            var result = await _userService.LoginAsync(request);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var token = _jwtTokenDecoderService.GetTokenFromHeaders(Request);
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Authorization header missing or invalid.");
            }
            try
            {
                var profile = await _userService.GetUserProfileAsync(token);
                return Ok(profile);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto request)
        {
            var result = await _userService.ForgotPasswordAsync(request);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request)
        {
            var result = await _userService.ResetPasswordAsync(request);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

    }
}
