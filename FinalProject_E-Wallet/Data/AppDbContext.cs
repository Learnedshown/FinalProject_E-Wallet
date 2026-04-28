using Microsoft.EntityFrameworkCore;
using FinalProject_E_Wallet.Models;
namespace FinalProject_E_Wallet.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
    }
}
