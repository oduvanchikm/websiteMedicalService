using CourseWorkDataBase.DAL.Configurations;
using Microsoft.EntityFrameworkCore;
using CourseWorkDataBase.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace CourseWorkDataBase.DAL;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Specialty> Specialties { get; set; }
    public DbSet<Clinic> Clinics { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<AppointmentSlot> AppointmentSlots { get; set; } 
    public DbSet<Status> Statuses { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new PatientConfiguration());
        modelBuilder.ApplyConfiguration(new DoctorConfiguration());
        modelBuilder.ApplyConfiguration(new SpecialtyConfiguration());
        modelBuilder.ApplyConfiguration(new ClinicConfiguration());
        modelBuilder.ApplyConfiguration(new StatusConfiguration());
        modelBuilder.ApplyConfiguration(new AppointmentSlotConfiguration());
        modelBuilder.ApplyConfiguration(new AppointmentConfiguration());
        
        
        
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<AppointmentSlot>(entity =>
        {
            entity.Property(e => e.StartTime)
                .HasColumnType("timestamp with time zone");
            entity.Property(e => e.EndTime)
                .HasColumnType("timestamp with time zone");
        });
    }
}