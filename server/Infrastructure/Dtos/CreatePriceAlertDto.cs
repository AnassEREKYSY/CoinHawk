namespace Infrastructure.Dtos
{
    public class CreatePriceAlertDto
    {
        public string CoinName { get; set; }
        public decimal TargetPrice { get; set; }
    }
}