using Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class ApplicationContext : IdentityDbContext<AppUser>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

        public DbSet<PriceAlert> PriceAlerts { get; set; }
        public DbSet<Coin> Coins { get; set; }
        public DbSet<PriceHistory> PriceHistories { get; set; }
        public DbSet<EmailNotification> EmailNotifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PriceAlert>()
                .HasOne(pa => pa.User)
                .WithMany(u => u.PriceAlerts)
                .HasForeignKey(pa => pa.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PriceAlert>()
                .HasOne(pa => pa.Coin)
                .WithMany(c => c.PriceAlerts)
                .HasForeignKey(pa => pa.CoinId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PriceHistory>()
                .HasOne(ph => ph.Coin)
                .WithMany(c => c.PriceHistories)
                .HasForeignKey(ph => ph.CoinId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EmailNotification>()
                .HasOne(en => en.PriceAlert)
                .WithMany(pa => pa.EmailNotifications)
                .HasForeignKey(en => en.AlertId)
                .OnDelete(DeleteBehavior.ClientNoAction);

            modelBuilder.Entity<EmailNotification>()
                .HasOne(en => en.User)
                .WithMany(u => u.EmailNotifications)
                .HasForeignKey(en => en.UserId)
                .OnDelete(DeleteBehavior.ClientNoAction);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
