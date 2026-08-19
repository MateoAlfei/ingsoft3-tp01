using AppGastos.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AppGastos.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Expense> Expenses => Set<Expense>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Email).IsRequired();
            entity.Property(u => u.PasswordHash).IsRequired();
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(c => c.Name).IsRequired();
            entity.Property(c => c.MonthlyBudget).HasColumnType("numeric(12,2)");

            entity.HasOne(c => c.User)
                .WithMany(u => u.Categories)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Expense>(entity =>
        {
            entity.Property(x => x.Amount).HasColumnType("numeric(12,2)");

            entity.HasOne(x => x.User)
                .WithMany(u => u.Expenses)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Restrict: refuerza a nivel de base la regla de negocio de no poder
            // borrar una categoría con gastos asociados.
            entity.HasOne(x => x.Category)
                .WithMany(c => c.Expenses)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
