namespace Infrastructure.Dtos
{
    public class CoinDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ApiSymbol { get; set; }
        public string Symbol { get; set; }
        public int MarketCapRank { get; set; }
        public string Thumb { get; set; }
        public string Large { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal TargetPrice { get; set; }
        public decimal MarketCap { get; set; }
        public decimal TotalVolume { get; set; }
        public List<decimal[]> MarketChart { get; set; }
    }
}
