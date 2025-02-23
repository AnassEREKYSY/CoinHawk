namespace Core.Entities
{
    public class PriceAlert
    {
        public int Id { get; set; }        
        public string UserId { get; set; }
        public int CoinId { get; set; }
        public decimal TargetPrice { get; set; }
        public DateTime AlertSetAt { get; set; }
        public DateTime? AlertTriggeredAt { get; set; } 
        public bool IsNotified { get; set; }
        public AppUser User { get; set; }
        public Coin Coin { get; set; }

        public ICollection<EmailNotification> EmailNotifications { get; set; }
    }
}
