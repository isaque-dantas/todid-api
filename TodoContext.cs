using Microsoft.EntityFrameworkCore;
using TodoAPI.Models;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;

namespace TodoAPI;

public class TodoContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();
    public DbSet<TodoList> TodoLists => Set<TodoList>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite("Data Source=TodoDb.db");
    }
}

// protected override void OnModelCreating(ModelBuilder modelBuilder)
// {
//     modelBuilder.Entity<User>(entity =>
//     {
//         entity.Property(e => e.Email)
//             .HasColumnType("VARCHAR(64)");
//         
//         entity.HasIndex(e => e.Email)
//             .IsUnique()
//             .HasDatabaseName("IX_Email")
//             .HasFilter("Email IS NOT NULL");
//
//         entity.Property(e => e.Username)
//             .HasColumnType("VARCHAR(32)");
//
//         entity.HasIndex(e => e.Username)
//             .IsUnique()
//             .HasDatabaseName("IX_Username")
//             .HasFilter("Username IS NOT NULL");
//     });
//
//     base.OnModelCreating(modelBuilder);
// }