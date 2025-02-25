using Infrastructure.Dtos;
using Infrastructure.IServices;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/coins")]
    public class CoinsController(ICoinService _coinService) : ControllerBase
    {

        [HttpGet("all")]
        public async Task<IActionResult> GetAllCoins()
        {
            var result = await _coinService.GetAllCoinsAsync();
            if (result == null || !result.Any())
                return NotFound("No coins available.");

            return Ok(result);
        }

        [HttpGet("coin-data/{coinName}")]
        public async Task<IActionResult> GetCoinData(string coinName)
        {
            try
            {
                var result = await _coinService.GetCoinDataByNameAsync(coinName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("coin-market-chart/{coinName}/market-chart")]
        public async Task<IActionResult> GetCoinMarketChart(string coinName, [FromQuery] int days)
        {
            try
            {
                var coinData = await _coinService.GetCoinDataByNameAsync(coinName);
                var marketChart = await _coinService.GetMarketChartByCoinIdAsync(coinData.Id, days);
                return Ok(marketChart);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("coin-details-info/{coinName}")]
        public async Task<IActionResult> GetCoinInfo(string coinName, [FromQuery] int days)
        {
            try
            {
                var result = await _coinService.GetCoinInfoAsync(coinName, days);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        
    }
}
