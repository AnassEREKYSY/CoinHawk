using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Infrastructure.Dtos;
using Infrastructure.IServices;
using Infrastructure.Responses;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services
{
    public class NewsService(HttpClient _httpClient, IConfiguration _configuration) : INewsService
    {
        public async Task<NewsArticleDto> GetNewsForCoinAsync(string coinName)
        {
            var apiKey = _configuration["NewsAPI:ApiKey"];
            string queryTerm = coinName.Contains(" ") 
                ? $"\"{coinName}\" crypto" 
                : $"{coinName} crypto";
            var encodedQuery = Uri.EscapeDataString(queryTerm);
            var requestUrl = $"https://newsapi.org/v2/everything?q={encodedQuery}&apiKey={apiKey}&sortBy=publishedAt&language=en&pageSize=5";
            Console.WriteLine("request to send"+ requestUrl);

            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Add("User-Agent", "YourAppName/1.0");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"NewsAPI error: {response.StatusCode} - {error}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            };

            var result = JsonSerializer.Deserialize<NewsApiResponse>(responseContent, options);
            var latestArticle = result?.Articles?
                .OrderByDescending(article => article.PublishedAt)
                .FirstOrDefault();
            return latestArticle;
        }


    }

}
