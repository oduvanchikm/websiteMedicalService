using Microsoft.EntityFrameworkCore;
using CourseWorkDataBase.Models;
using CourseWorkDataBase.Helpers;

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
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<AppointmentSlot> AppointmentSlots { get; set; } 
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Admin" },        
            new Role { Id = 2, Name = "Doctor" },      
            new Role { Id = 3, Name = "Patient" }
        );
        
        modelBuilder.Entity<AppointmentSlot>(entity =>
        {
            entity.Property(e => e.StartTime)
                .HasColumnType("timestamp with time zone");
            entity.Property(e => e.EndTime)
                .HasColumnType("timestamp with time zone");
        });


        // var hasher = PasswordHelper.HashPassword("20012005", "mkgubareva2005@gmail.com");
        // modelBuilder.Entity<User>().HasData(
        //     new User
        //     {
        //         Id = 1, 
        //         Email = "mkgubareva2005@gmail.com",
        //         Password = "20012005", 
        //         CreatedAt = DateTime.UtcNow,
        //         RoleId = 1
        //     }
        // );
    }
}