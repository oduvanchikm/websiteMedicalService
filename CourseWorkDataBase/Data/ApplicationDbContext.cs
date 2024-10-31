using Microsoft.EntityFrameworkCore;
using CourseWorkDataBase.Models;

namespace CourseWorkDataBase.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Users> Users { get; set; }
    public DbSet<Roles> Roles { get; set; }
    public DbSet<Doctors> Doctors { get; set; }
    public DbSet<Specialties> Specialties { get; set; }
    public DbSet<Appointments> Appointments { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Roles>()
            .HasIndex(r => r.Name)
            .IsUnique();
        
        modelBuilder.Entity<Doctors>()
            .HasOne(d => d.Specialty)
            .WithMany(s => s.Doctors)
            .HasForeignKey(d => d.SpecialtyID)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Appointments>()
            .HasOne(a => a.Doctor)
            .WithMany(d => d.Appointments)
            .HasForeignKey(a => a.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Appointments>()
            .HasOne(a => a.User)
            .WithMany(u => u.Appointments)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Users>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}