using CourseWorkDataBase.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseWorkDataBase.DAL.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Date)
            .IsRequired();
        
        builder.HasOne(a => a.Patient)
            .WithMany(p => p.Appointments)
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(a => a.AppointmentSlot)
            .WithOne(a => a.Appointment)
            .HasForeignKey<Appointment>(a => a.AppointmentSlotId)
            .IsRequired() 
            .OnDelete(DeleteBehavior.Cascade); 
        
        builder.HasIndex(a => a.AppointmentSlotId)
            .IsUnique();
        
        builder.HasOne(x => x.Status)
            .WithMany(r => r.Appointments)
            .HasForeignKey(x => x.StatusId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict); 
        
        builder.HasMany(a => a.MedicalRecords)
            .WithOne(m => m.Appointment)
            .HasForeignKey(m => m.AppointmentId)
            .OnDelete(DeleteBehavior.SetNull);

    }
}