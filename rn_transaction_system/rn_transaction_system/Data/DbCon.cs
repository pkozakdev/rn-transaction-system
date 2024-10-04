using Microsoft.EntityFrameworkCore;
using rn_transaction_system.Entities;

namespace rn_transaction_system.Data
{
    public class DbCon : DbContext
    {
        public DbSet<Account> Accounts { get; set; }

        public DbCon(DbContextOptions<DbCon> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Balance)
                      .IsRequired()
                      .HasColumnType("decimal(18,2)");

                entity.Property(e => e.UserId)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.RowVersion)
                      .IsRowVersion();
            });

            // Seed initial data
            modelBuilder.Entity<Account>().HasData(
                new Account { Id = 1, Balance = 1000, UserId = "user1", RowVersion = new byte[] { } },
                new Account { Id = 2, Balance = 2000, UserId = "user2", RowVersion = new byte[] { } },
                new Account { Id = 3, Balance = 3000, UserId = "user3", RowVersion = new byte[] { } },
                new Account { Id = 4, Balance = 4000, UserId = "user4", RowVersion = new byte[] { } },
                new Account { Id = 5, Balance = 5000, UserId = "user5", RowVersion = new byte[] { } }
            );
        }
    }
}
