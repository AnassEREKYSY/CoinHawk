using Infrastructure.Dtos;
using Core.IServices;

namespace Core.IServices
{
    public interface IUserService
    {
        Task<AuthenticationResultDto> RegisterAsync(RegisterDto request);
        Task<AuthenticationResultDto> LoginAsync(LoginDto request);
    }
}
