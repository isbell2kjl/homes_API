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
           entity.HasKey(c => c.ProjectId);
           entity.Property (c => c.ProjectName).IsRequired();
           entity.Property (c => c.Description);
           entity.Property(c => c.MainTitle); 
           entity.Property(c => c.MainText);
           entity.Property(c => c.Tagline);
           entity.Property(c => c.LeftTitle);
           entity.Property(c => c.LeftText);
           entity.Property(c => c.CenterTitle);
           entity.Property(c => c.CenterText);
           entity.Property(c => c.RightTitle);
           entity.Property(c => c.RightText);
           entity.Property(c => c.ContactText);
           entity.Property(c => c.ContactEmail);
           entity.Property(c => c.ContactPhone);
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
            entity.HasOne(p => p.Project)
            .WithMany(u => u.Users)
            .HasForeignKey(p => p.ProjId_fk);

        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(p => p.PostId);
            entity.Property(p => p.Title);
            entity.Property(p => p.PhotoURL);
            entity.Property(p => p.Content).IsRequired();
            entity.Property(p => p.Posted);
            entity.Property(p => p.Visible);
            entity.Property(p => p.Archive);
            entity.HasOne(p => p.User)
            .WithMany(u => u.Posts)
            .HasForeignKey(p => p.UserId_fk);
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(c => c.ComId);
            entity.Property(c => c.Task);
            entity.Property(c => c.Text).IsRequired();
            entity.Property(c => c.ComDate);
            entity.HasOne(c => c.Post)
            .WithMany(p => p.Comments)
            .HasForeignKey(c => c.PostId_fk);
            entity.HasOne(c => c.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UsrId_fk);
        });

        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}

