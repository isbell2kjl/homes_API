using homes_API.Models;
using Microsoft.EntityFrameworkCore;


namespace homes_API.Migrations;

public class PostDbContext : DbContext
{
    public DbSet<Project>? Projects { get; set; }
    public DbSet<User>? Users { get; set; }
    public DbSet<Post>? Posts { get; set; }
    public DbSet<Comment>? Comments { get; set; }

    protected readonly IConfiguration Configuration;

    public PostDbContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // connect to mysql with connection string from app settings
        var connectionString = Configuration.GetConnectionString("Default");
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Project>(entity =>
       {
           entity.HasKey(r => r.ProjectId);
           entity.Property(r => r.ProjectName).IsRequired();
           entity.Property(r => r.SiteName); 
           entity.Property(r => r.MainTitle); 
           entity.Property(r => r.MainText);
           entity.Property(r => r.Tagline);
           entity.Property(r => r.LeftTitle);
           entity.Property(r => r.LeftText);
           entity.Property(r => r.CenterTitle);
           entity.Property(r => r.CenterText);
           entity.Property(r => r.RightTitle);
           entity.Property(r => r.RightText);
           entity.Property(r => r.ContactText);
           entity.Property(r => r.ContactEmail).IsRequired();
           entity.Property(r => r.ContactPhone);
       });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.UserId);
            entity.Property(u => u.UserName).IsRequired();
            entity.HasIndex(x => x.UserName).IsUnique();
            entity.Property(u => u.Password).IsRequired();
            entity.Property(u => u.Email).IsRequired();
            entity.HasIndex(x => x.Email).IsUnique();
            entity.Property(u => u.FirstName);
            entity.Property(u => u.LastName);
            entity.Property(u => u.City);
            entity.Property(u => u.State);
            entity.Property(u => u.Country);
            entity.Property(u => u.Created);
            entity.Property(u => u.ResetToken);
            entity.Property(u => u.ResetTokenExpires);
            entity.Property(u => u.RefreshToken);
            entity.Property(u => u.RefreshTokenExpires);
            entity.Property(u => u.Terms);
            entity.Property(u => u.Privacy);
            entity.Property(u => u.Role);
            entity.HasOne(p => p.Project)
            .WithMany(u => u.Users)
            .HasForeignKey(u => u.ProjId_fk).IsRequired();

        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(p => p.PostId);
            entity.Property(p => p.Title).IsRequired();
            entity.Property(p => p.PhotoURL);
            entity.Property(p => p.Content).IsRequired();
            entity.Property(p => p.Posted);
            entity.Property(p => p.Visible);
            entity.Property(p => p.Archive);
            entity.HasOne(u => u.User)
            .WithMany(p => p.Posts)
            .HasForeignKey(p => p.UserId_fk);
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(c => c.ComId);
            entity.Property(c => c.Text).IsRequired();
            entity.Property(c => c.ComDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(c => c.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");
            entity.HasOne(p => p.Post)
            .WithMany(c => c.Comments)
            .HasForeignKey(c => c.PostId_fk);
            entity.HasOne(u => u.User)
            .WithMany(c => c.Comments)
            .HasForeignKey(c => c.UsrId_fk);
        });

        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}

