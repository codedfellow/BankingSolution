using Application.Contracts.Data;
using Domain.Entitites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Infrastructure.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IAppDbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        private IDbContextTransaction? _currentTransaction;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            modelBuilder.Entity<Account>()
                .HasIndex(e => e.AccountNumber)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(e => e.Email)
                .IsUnique();

            modelBuilder.Entity<Account>()
                .Property(a => a.AccountBalance)
                .HasPrecision(18, 4);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasPrecision(18, 4);

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasOne(t => t.SenderRef)
                      .WithMany()
                      .HasForeignKey(t => t.SenderId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.ReceiverRef)
                      .WithMany()
                      .HasForeignKey(t => t.ReceiverId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasOne(a => a.UserRef)
                      .WithMany()
                      .HasForeignKey(a => a.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync(CancellationToken ct)
        {
            if (_currentTransaction != null)
                return;

            _currentTransaction =
                await Database.BeginTransactionAsync(ct);
        }

        public async Task CommitTransactionAsync(CancellationToken ct)
        {
            try
            {
                await SaveChangesAsync(ct);
                await _currentTransaction!.CommitAsync(ct);
            }
            catch
            {
                await RollbackTransactionAsync(ct);
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                    await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken ct)
        {
            try
            {
                await _currentTransaction?.RollbackAsync(ct)!;
            }
            finally
            {
                if (_currentTransaction != null)
                    await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
    }
}
