using FinTracker.Models.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FinTracker.Models
{
    public partial class FintrackContext : IdentityDbContext<ApplicationUser>
    {
        public FintrackContext()
        {
        }

        public FintrackContext(DbContextOptions<FintrackContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Budget> Budget { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Salary> Salaries { get; set; }
        public virtual DbSet<Summary> Summaries { get; set; }
        public virtual DbSet<Summaryosn> Summaryosns { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<BudgetSnapshots> BudgetSnapshots { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройки для SQL Server

            // Employee
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.Property(e => e.Salary)
                      .HasColumnType("decimal(18,2)"); // Для SQL Server оставляем как есть
            });

            // Salary
            modelBuilder.Entity<Salary>(entity =>
            {
                entity.Property(e => e.Amount)
                      .HasColumnType("decimal(18,2)");
            });

            // Summary
            modelBuilder.Entity<Summary>(entity =>
            {
                entity.Property(e => e.Revenues).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Expenses).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Usn6).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Usn15).HasColumnType("decimal(18,2)");
                entity.Property(e => e.NetIncome).HasColumnType("decimal(18,2)");
            });

            // Summaryosn
            modelBuilder.Entity<Summaryosn>(entity =>
            {
                entity.Property(e => e.Revenues).HasColumnType("decimal(18,2)");
                entity.Property(e => e.AllExpenses).HasColumnType("decimal(18,2)");
                entity.Property(e => e.MaterialExpenses).HasColumnType("decimal(18,2)");
                entity.Property(e => e.AccruedNds).HasColumnType("decimal(18,2)");
                entity.Property(e => e.InputNds).HasColumnType("decimal(18,2)");
                entity.Property(e => e.PropertyTax).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalOsn).HasColumnType("decimal(18,2)");
            });

            // Transaction
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.Property(e => e.Amount)
                      .HasColumnType("decimal(18,2)");
            });

            // Budget
            modelBuilder.Entity<Budget>(entity =>
            {
                entity.Property(e => e.BudgetAmount)
                      .HasColumnType("decimal(18,2)");
            });

            // BudgetSnapshots
            modelBuilder.Entity<BudgetSnapshots>(entity =>
            {
                entity.Property(b => b.Balance)
                      .HasColumnType("decimal(18,2)");
                entity.Property(b => b.Date)
                    .HasColumnType("datetime2");
            });

            // Identity configuration
            base.OnModelCreating(modelBuilder);

            // Настройка связи пользователь-сотрудник
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.HasOne(u => u.Employee)
                    .WithMany()
                    .HasForeignKey(u => u.EmployeeId)
                    .OnDelete(DeleteBehavior.SetNull) // SET NULL для SQL Server
                    .IsRequired(false); // Разрешаем NULL
            });

            // Настройки длин для Identity
            modelBuilder.Entity<IdentityRole>(entity =>
            {
                entity.Property(e => e.Name)
                    .HasMaxLength(256);
                entity.Property(e => e.NormalizedName)
                    .HasMaxLength(256);
                entity.Property(e => e.ConcurrencyStamp)
                    .HasMaxLength(256); // Увеличиваем до 256 для SQL Server
            });

            modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.Property(e => e.LoginProvider)
                    .HasMaxLength(128);
                entity.Property(e => e.ProviderKey)
                    .HasMaxLength(128);
            });

            modelBuilder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.Property(e => e.LoginProvider)
                    .HasMaxLength(128);
                entity.Property(e => e.Name)
                    .HasMaxLength(128);
            });
        }

    }
}
