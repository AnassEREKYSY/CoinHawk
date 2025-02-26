using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.Dtos;

namespace Infrastructure.IServices
{
    public interface INewsService
    {
        Task<IEnumerable<NewsArticleDto>> GetNewsForCoinAsync(string coinName);
    }
}
