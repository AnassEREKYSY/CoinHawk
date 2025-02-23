using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Infrastructure.Dtos;
using Core.Entities;
using Core.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services
{
    public class UserService(UserManager<AppUser> _userManager, IConfiguration _configuration) : IUserService
    {
        public async Task<AuthenticationResultDto> RegisterAsync(RegisterDto request)
        {
            var user = new AppUser
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                EmailConfirmed = true
            };

            var identityResult = await _userManager.CreateAsync(user, request.Password);
            if (!identityResult.Succeeded)
            {
                return new AuthenticationResultDto
                {
                    Success = false,
                    Errors = identityResult.Errors.Select(e => e.Description)
                };
            }
            return new AuthenticationResultDto
            {
                Success = true,
                Token = null
            };
        }

        public async Task<AuthenticationResultDto> LoginAsync(LoginDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return new AuthenticationResultDto
                {
                    Success = false,
                    Errors = ["User does not exist"]
                };
            }

            var validPassword = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!validPassword)
            {
                return new AuthenticationResultDto
                {
                    Success = false,
                    Errors = ["Invalid credentials"]
                };
            }

            return await GenerateAuthenticationResultForUserAsync(user);
        }

        private async Task<AuthenticationResultDto> GenerateAuthenticationResultForUserAsync(AppUser user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);
            var now = DateTime.UtcNow;
            var expiryMinutes = Convert.ToDouble(jwtSettings["ExpiryInMinutes"]);
            if(expiryMinutes <= 0)
            {
                expiryMinutes = 60;
            }
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim("id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                NotBefore = now,
                Expires = now.AddMinutes(expiryMinutes),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new AuthenticationResultDto
            {
                Success = true,
                Token = tokenHandler.WriteToken(token)
            };
        }
    }
}
