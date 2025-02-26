using Infrastructure.Dtos;

namespace Infrastructure.IServices
{
    public interface INewsService
    {
        Task<NewsArticleDto> GetNewsForCoinAsync(string coinName);
    }
}
