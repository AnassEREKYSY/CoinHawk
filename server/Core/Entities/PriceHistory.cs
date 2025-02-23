namespace Core.Entities
{
    public class PriceHistory
    {
        public int Id { get; set; }
        public int CoinId { get; set; }
        public DateTime Timestamp { get; set; }
        public decimal Price { get; set; }
        public Coin Coin { get; set; }
    }
}
