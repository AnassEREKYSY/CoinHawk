using Infrastructure.Dtos

namespace Infrastructure.Responses
{
    public class NewsApiResponse
    {
        public string Status { get; set; }
        public int TotalResults { get; set; }
        public IEnumerable<NewsArticleDto> Articles { get; set; }
    }
}