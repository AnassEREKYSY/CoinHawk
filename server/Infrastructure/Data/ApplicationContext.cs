using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class ApplicationContext(DbContextOptions<ApplicationContext> options) : DbContext(options)
    {
        public DbSet<PriceAlert> PriceAlerts { get; set; }
        public DbSet<PriceHistory> PriceHistories { get; set; }
        public DbSet<EmailNotification> EmailNotifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PriceAlert>()
                .Property(pa => pa.UserEmail)
                .IsRequired();

            modelBuilder.Entity<PriceAlert>()
                .HasMany(pa => pa.EmailNotifications)
                .WithOne()
                .HasForeignKey(en => en.AlertId)
                .OnDelete(DeleteBehavior.ClientNoAction);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
