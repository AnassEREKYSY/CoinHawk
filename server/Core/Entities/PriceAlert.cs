using System.Text.Json.Serialization;

namespace Core.Entities
{
    public class PriceAlert
    {
        public int Id { get; set; }        
        public string UserId { get; set; }
        public string CoinName { get; set; }
        public string coinId { get; set; }
        public decimal TargetPrice { get; set; }
        public DateTime AlertSetAt { get; set; }
        public DateTime? AlertTriggeredAt { get; set; } 
        public bool IsNotified { get; set; }

        [JsonIgnore]
        public AppUser User { get; set; }
        public ICollection<EmailNotification> EmailNotifications { get; set; }
    }
}
