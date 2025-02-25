namespace Infrastructure.Dtos
{
    public class CreatePriceAlertDto
    {
        public int CoinId { get; set; }
        public decimal TargetPrice { get; set; }
    }
}