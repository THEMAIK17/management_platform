using Microsoft.EntityFrameworkCore;
using Platform.Domain.Entities;

namespace Platform.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Project> Projects { get; set; }
    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Project configuration
        builder.Entity<Project>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(p => p.Description)
                .HasMaxLength(1000);

            entity.Property(p => p.Status)
                .IsRequired()
                .HasConversion<string>(); // Stores "Draft", "Active", "Completed" as strings
        });

        // TaskItem configuration
        builder.Entity<TaskItem>(entity =>
        {
            entity.HasKey(t => t.Id);

            entity.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(300);

            entity.Property(t => t.Priority)
                .IsRequired()
                .HasConversion<string>(); // Stores "Low", "Medium", "High" as strings

            entity.Property(t => t.Order)
                .IsRequired();

            // Relationship: TaskItem belongs to Project (Cascade Delete)
            entity.HasOne(t => t.Project)
                .WithMany(p => p.Tasks)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique constraint: Order must be unique within a project
            entity.HasIndex(t => new { t.ProjectId, t.Order })
                .IsUnique();
        });

        // User configuration
        builder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(256);

            entity.HasIndex(u => u.Email)
                .IsUnique(); // No duplicate emails

            entity.Property(u => u.PasswordHash)
                .IsRequired();
        });
    }
}
