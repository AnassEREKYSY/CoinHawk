using Infrastructure.Dtos;
using Infrastructure.IServices;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class CoinsController(ICoinService _coinService) : ControllerBase
    {
        
        [HttpGet("getAll/{coinName}")]
        public async Task<IActionResult> GetAll(string coinName)
        {
            var result = await _coinService.SearchAndDisplayCoinsAsync(coinName);
            if (result == null || !result.Any())
                return NotFound("No coins found matching the search criteria.");

            return Ok(result);
        }
        
    }
}
