using blog_API.Models;
using Microsoft.EntityFrameworkCore;

namespace blog_API.Migrations;

public class PostDbContext : DbContext
{
    public DbSet<Post>? Posts { get; set; }
    public DbSet<User>? Users { get; set; }
    public PostDbContext(DbContextOptions<PostDbContext> options)
        : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(p => p.PostId);
            entity.Property(p => p.Title);
            entity.Property(p => p.Content).IsRequired();
            entity.Property(p => p.Posted);
            entity.HasOne(p => p.User)
            .WithMany(u => u.Posts)
            .HasForeignKey(p => p.UserId_fk);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.UserId);
            entity.Property(u => u.UserName).IsRequired();
            entity.HasIndex(x => x.UserName).IsUnique();
            entity.Property(u => u.Password).IsRequired();
            entity.Property(u => u.Email);
            entity.HasIndex(x => x.Email).IsUnique();
            entity.Property(u => u.FirstName);
            entity.Property(u => u.LastName);
            entity.Property(u => u.City);
            entity.Property(u => u.State);
            entity.Property(u => u.Country);
            entity.Property(u => u.Created);
            
        });
    }
}