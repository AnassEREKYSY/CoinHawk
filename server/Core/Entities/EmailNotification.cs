namespace Core.Entities
{
    public class EmailNotification
    {
        public int Id { get; set; }        
        public string UserEmail { get; set; }        
        public int AlertId { get; set; }
        public DateTime EmailSentAt { get; set; }
        public string EmailStatus { get; set; }
    }
}
