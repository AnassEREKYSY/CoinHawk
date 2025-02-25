namespace Infrastructure.Dtos
{
    public class PriceAlertDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string CoinId { get; set; }
        public decimal TargetPrice { get; set; }
        public DateTime AlertSetAt { get; set; }
        public DateTime? AlertTriggeredAt { get; set; }
        public bool IsNotified { get; set; }        
        public CoinDto Coin { get; set; }
    }
}
