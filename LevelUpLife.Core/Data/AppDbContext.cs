using LevelUpLife.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace LevelUpLife.Core.Data;

/// <summary>
/// Контекст базы данных приложения (Entity Framework Core + SQLite)
/// Описывает таблицы, ограничения и начальные данные
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    public DbSet<User> Users => Set<User>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Habit> Habits => Set<Habit>();
    public DbSet<HabitLog> HabitLogs => Set<HabitLog>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("users");
            e.Property(u => u.Username).IsRequired().HasMaxLength(30);
            e.Property(u => u.PasswordHash).IsRequired().HasMaxLength(255);
            e.Property(u => u.DisplayName).IsRequired().HasMaxLength(30);
            e.HasIndex(u => u.Username).IsUnique();
            e.HasIndex(u => u.TotalXp);

            e.HasMany(u => u.Sessions)
                .WithOne(s => s.User)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasMany(u => u.Habits)
                .WithOne(h => h.User)
                .HasForeignKey(h => h.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Session>(e =>
        {
            e.ToTable("sessions");
            e.Property(s => s.Token).IsRequired().HasMaxLength(64);
            e.HasIndex(s => s.Token).IsUnique();
        });

        modelBuilder.Entity<Category>(e =>
        {
            e.ToTable("categories");
            e.Property(c => c.Name).IsRequired().HasMaxLength(50);
            e.HasIndex(c => c.Name).IsUnique();

            // Категорию с привычками удалить нельзя.
            e.HasMany(c => c.Habits)
                .WithOne(h => h.Category)
                .HasForeignKey(h => h.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasData(
                new Category { Id = 1, Name = "Спорт" },
                new Category { Id = 2, Name = "Учёба" },
                new Category { Id = 3, Name = "Режим дня" },
                new Category { Id = 4, Name = "Здоровье" },
                new Category { Id = 5, Name = "Саморазвитие" });
        });

        modelBuilder.Entity<Habit>(e =>
        {
            e.ToTable("habits");
            e.Property(h => h.Title).IsRequired().HasMaxLength(100);
            e.Property(h => h.DaysOfWeek).IsRequired().HasMaxLength(7);
            e.Property(h => h.Difficulty).HasConversion<string>().HasMaxLength(10);

            e.HasMany(h => h.Logs)
                .WithOne(l => l.Habit)
                .HasForeignKey(l => l.HabitId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<HabitLog>(e =>
        {
            e.ToTable("habit_logs");
            e.Property(l => l.Status).HasConversion<string>().HasMaxLength(15);

            // Одну привычку нельзя отметить дважды за один день
            e.HasIndex(l => new { l.HabitId, l.LogDate }).IsUnique();
        });
    }
}
