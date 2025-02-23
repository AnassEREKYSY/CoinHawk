using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class ApplicationContext : IdentityDbContext<AppUser>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

        // public DbSet<Budget> Budgets { get; set; }
        // public DbSet<Category> Categories { get; set; }
        // public DbSet<ExportRequest> ExportRequests { get; set; }
        // public DbSet<Notification> Notifications { get; set; }
        // public DbSet<Transaction> Transactions { get; set; }

        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }
    }
}