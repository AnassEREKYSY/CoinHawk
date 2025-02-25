namespace Infrastructure.Dtos
{
    public class CreatePriceAlertDto
    {
        public string UserToken { get; set; }
        public int CoinId { get; set; }
        public decimal TargetPrice { get; set; }
    }
}