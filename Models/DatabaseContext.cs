using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace BankingVault.Models
{
    public class DatabaseContext:DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> context) : base(context)
        {
            
        }
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<AccountType> AccountTypes { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<TransactionRecord> TransactionsRecords { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TransactionRecord>(entity =>
            {
                entity.HasMany(trans => trans.Transactions)
                .WithOne(Re => Re.Record)
                .HasForeignKey(Re => Re.RecordID)
                .IsRequired();
            });

            modelBuilder.Entity<AccountType>(entity =>
            {
                entity.HasOne(Re=>Re.Record)
                .WithOne(U=>U.AccountContext)
                .HasForeignKey<TransactionRecord>(U=> U.AccountContextID)
                .IsRequired();
            });

            modelBuilder.Entity<AccountType>(entity =>
            {
                entity.Property(entity => entity.Context)
                .HasConversion<string>();
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.Property(entity=>entity.TransactionContext)
                .HasConversion<string>();
            });
        }
    }
}
