using Infrastructure.Dtos;

namespace Infrastructure.IServices
{
    public interface IUserService
    {
        Task<AuthenticationResultDto> RegisterAsync(RegisterDto request);
        Task<AuthenticationResultDto> LoginAsync(LoginDto request);
    }
}
