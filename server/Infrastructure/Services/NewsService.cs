using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Infrastructure.Dtos;
using Infrastructure.IServices;
using Infrastructure.Responses;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services
{
    public class NewsService(HttpClient _httpClient, IConfiguration _configuration) : INewsService
    {

        public async Task<IEnumerable<NewsArticleDto>> GetNewsForCoinAsync(string coinName)
        {
            var apiKey = _configuration["NewsAPI:ApiKey"];
            var requestUrl = $"https://newsapi.org/v2/everything?q={coinName}&apiKey={apiKey}&sortBy=publishedAt";
            
            var response = await _httpClient.GetAsync(requestUrl);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to fetch news articles.");
            }
            
            var result = await response.Content.ReadFromJsonAsync<NewsApiResponse>();
            return result?.Articles;
        }
    }

}
