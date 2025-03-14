namespace Core.Entities
{
    public class PriceAlert
    {
        public int Id { get; set; }
        public string UserEmail { get; set; }
        public string CoinName { get; set; }
        public string CoinId { get; set; }
        public decimal TargetPrice { get; set; }
        public DateTime AlertSetAt { get; set; }
        public DateTime? AlertTriggeredAt { get; set; } 
        public bool IsNotified { get; set; }
        public ICollection<EmailNotification> EmailNotifications { get; set; }
    }
}
