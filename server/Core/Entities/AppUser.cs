using Microsoft.AspNetCore.Identity;

namespace Core.Entities
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }  = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public ICollection<PriceAlert> PriceAlerts { get; set; }
        public ICollection<EmailNotification> EmailNotifications { get; set; }
    }
}
