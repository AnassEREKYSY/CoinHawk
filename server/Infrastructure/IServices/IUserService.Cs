using Infrastructure.Dtos;

namespace Infrastructure.IServices
{
    public interface IUserService
    {
        Task<AuthenticationResultDto> RegisterAsync(RegisterDto request);
        Task<AuthenticationResultDto> LoginAsync(LoginDto request);
        Task<UserProfileDto> GetUserProfileAsync(string token);
        Task<AuthenticationResultDto> ForgotPasswordAsync(ForgotPasswordDto request);
        Task<AuthenticationResultDto> ResetPasswordAsync(ResetPasswordDto request);
    }
}
