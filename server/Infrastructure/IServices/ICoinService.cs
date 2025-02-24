using Infrastructure.Dtos;

namespace Infrastructure.IServices
{
    public interface ICoinService
    {
        Task<List<CoinDto>> SearchAndDisplayCoinsAsync(string coinName);
    }
}
