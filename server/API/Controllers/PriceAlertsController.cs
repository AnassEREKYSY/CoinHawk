using Infrastructure.IServices;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Infrastructure.Dtos

namespace API.Controllers
{
    [ApiController]
    [Route("api/price-alerts")]
    public class PriceAlertsController(IPriceAlertService _priceAlertService) : ControllerBase
    {

        [HttpPost("create")]
        public async Task<IActionResult> CreatePriceAlert([FromBody] CreatePriceAlertDto createAlertDto)
        {
            try
            {
                var alert = await _priceAlertService.CreatePriceAlertAsync(
                    createAlertDto.UserToken,
                    createAlertDto.CoinId,
                    createAlertDto.TargetPrice);

                return Ok(alert);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
