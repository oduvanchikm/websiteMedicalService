using Microsoft.EntityFrameworkCore;
using CourseWorkDataBase.Models;

namespace CourseWorkDataBase.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Specialty> Specialties { get; set; }
    public DbSet<Clinic> Clinics { get; set; }
    // public DbSet<Appointments> Appointments { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>().HasData(new Role { Id = 1, Name = "Patient" });
        // base.OnModelCreating(modelBuilder);
        //
        // modelBuilder.Entity<Roles>().HasData(
        //     new Roles { Id = -1, Name = "Admin" },
        //     new Roles { Id = -2, Name = "Patient" },
        //     new Roles { Id = -3, Name = "Doctor" }
        // );
        //
        // modelBuilder.Entity<Doctor>()
        //     .HasOne(d => d.Specialty)
        //     .WithMany(s => s.Doctors)
        //     .HasForeignKey(d => d.SpecialtyID)
        //     .OnDelete(DeleteBehavior.Restrict);
        //
        // modelBuilder.Entity<Appointments>()
        //     .HasOne(a => a.Doctor)
        //     .WithMany(d => d.Appointments)
        //     .HasForeignKey(a => a.DoctorId)
        //     .OnDelete(DeleteBehavior.Restrict);
        //
        // modelBuilder.Entity<Appointments>()
        //     .HasOne(a => a.User)
        //     .WithMany(u => u.Appointments)
        //     .HasForeignKey(a => a.UserId)
        //     .OnDelete(DeleteBehavior.Restrict);
        //
        // modelBuilder.Entity<User>()
        //     .HasOne(u => u.Role)
        //     .WithMany(r => r.)
        //     .HasForeignKey(u => u.RoleId)
        //     .OnDelete(DeleteBehavior.Restrict);
    }
}