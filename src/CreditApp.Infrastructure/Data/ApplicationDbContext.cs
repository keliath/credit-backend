using CreditApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using CreditApp.Infrastructure.Data.Configurations;

namespace CreditApp.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<CreditRequest> CreditRequests { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Money value object
            modelBuilder.ConfigureMoney();

            // Configure User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.Role).IsRequired();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Username).IsUnique();
            });

            // Configure CreditRequest
            modelBuilder.Entity<CreditRequest>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Purpose).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
                entity.Property(e => e.RejectionReason).HasMaxLength(500);
                entity.Property(e => e.ApprovedBy).HasMaxLength(50);

                entity.HasOne(cr => cr.User)
                    .WithMany(u => u.CreditRequests)
                    .HasForeignKey(cr => cr.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.EntityName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Action).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Details).HasMaxLength(1000);
                entity.Property(e => e.PerformedBy).IsRequired().HasMaxLength(50);
            });
        }

        public async Task<IEnumerable<CreditRequest>> GetCreditRequestsByUserAsync(Guid userId)
        {
            return await CreditRequests
                .FromSqlRaw("EXEC [dbo].[GetCreditRequestsByUser] @UserId",
                    new Microsoft.Data.SqlClient.SqlParameter("@UserId", userId))
                .ToListAsync();
        }

        public async Task UpdateCreditRequestStatusAsync(
            Guid creditRequestId,
            string newStatus,
            string rejectionReason = null!,
            string approvedBy = null!)
        {
            await Database.ExecuteSqlRawAsync(
                "EXEC [dbo].[UpdateCreditRequestStatus] @CreditRequestId, @NewStatus, @RejectionReason, @ApprovedBy",
                new Microsoft.Data.SqlClient.SqlParameter("@CreditRequestId", creditRequestId),
                new Microsoft.Data.SqlClient.SqlParameter("@NewStatus", newStatus),
                new Microsoft.Data.SqlClient.SqlParameter("@RejectionReason", (object?)rejectionReason ?? DBNull.Value),
                new Microsoft.Data.SqlClient.SqlParameter("@ApprovedBy", (object?)approvedBy ?? DBNull.Value));
        }
    }
}