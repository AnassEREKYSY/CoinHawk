namespace Core.Entities
{
    public class EmailNotification
    {
        public int Id { get; set; }        
        public string UserId { get; set; }        
        public int AlertId { get; set; }
        public DateTime EmailSentAt { get; set; }
        public string EmailStatus { get; set; }
        public AppUser User { get; set; }
        public PriceAlert PriceAlert { get; set; }
    }
}
