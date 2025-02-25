namespace Infrastructure.IServices
{
    public interface IEmailNotificationService
    {
        Task SendPriceAlertEmailAsync(string email, string coinName, decimal currentPrice, decimal targetPrice);
    }
}
