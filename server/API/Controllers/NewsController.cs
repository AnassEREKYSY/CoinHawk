using Infrastructure.Dtos;
using Infrastructure.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [ApiController]
    [Route("api/news")]
    [Authorize]
    public class NewsController(INewsService _newsService) : ControllerBase
    {

        [HttpGet("coin/{coinName}")]
        public async Task<IActionResult> GetNewsForCoin(string coinName)
        {
            try
            {
                var newsArticles = await _newsService.GetNewsForCoinAsync(coinName);
                return Ok(newsArticles);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

}
