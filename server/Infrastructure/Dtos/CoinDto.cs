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
    }
}