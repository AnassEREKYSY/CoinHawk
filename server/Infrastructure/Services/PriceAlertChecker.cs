using Core.Entities;
using Infrastructure.IServices;
using Infrastructure.Data; // Assuming ApplicationDbContext is here
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class PriceAlertChecker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<PriceAlertChecker> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);

        public PriceAlertChecker(IServiceScopeFactory scopeFactory, ILogger<PriceAlertChecker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("PriceAlertChecker started.");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
                        var coinService = scope.ServiceProvider.GetRequiredService<ICoinService>();
                        var emailService = scope.ServiceProvider.GetRequiredService<IEmailNotificationService>();

                        var alerts = await dbContext.PriceAlerts
                            .Include(a => a.User)
                            .Where(a => !a.IsNotified)
                            .ToListAsync(stoppingToken);

                        foreach (var alert in alerts)
                        {
                            try
                            {
                                var coinInfo = await coinService.GetCoinInfoAsync(alert.CoinName, 1); 
                                if (coinInfo.CurrentPrice >= alert.TargetPrice)
                                {
                                    await emailService.SendPriceAlertEmailAsync(
                                        alert.User.Email,
                                        coinInfo.Name,
                                        coinInfo.CurrentPrice,
                                        alert.TargetPrice);

                                    alert.IsNotified = true;
                                    alert.AlertTriggeredAt = DateTime.UtcNow;
                                    dbContext.PriceAlerts.Update(alert);
                                    await dbContext.SaveChangesAsync(stoppingToken);
                                    _logger.LogInformation($"Alert {alert.Id} triggered and notification sent.");
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, $"Error processing alert {alert.Id}.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while checking price alerts.");
                }
                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("PriceAlertChecker is stopping.");
        }
    }
}
