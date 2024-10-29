using Microsoft.EntityFrameworkCore;
using CourseWorkDataBase.Models;

namespace CourseWorkDataBase.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Users> User { get; set; }
    public DbSet<Roles> Role { get; set; }
    
    // protected override void OnModelCreating(ModelBuilder modelBuilder)
    // {
    //     base.OnModelCreating(modelBuilder);
    //     
    //     modelBuilder.Entity<Users>()
    //         .HasOne(u => u.Role)
    //         .WithMany(r => r.Users)
    //         .HasForeignKey(u => u.Role)
    //         .OnDelete(DeleteBehavior.Restrict); 
    // }
    //
    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     if (!optionsBuilder.IsConfigured)
    //     {
    //         optionsBuilder.UseNpgsql("YourConnectionStringHere");
    //     }
    // }
}