using CourseWorkDataBase.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseWorkDataBase.DAL.Configurations;

public class MedicalRecordsConfiguration : IEntityTypeConfiguration<MedicalRecords>
{
    public void Configure(EntityTypeBuilder<MedicalRecords> builder)
    {
        builder.HasKey(a => a.Id);
        
        builder.Property(a => a.Diagnosis)
            .IsRequired();
        
        builder.Property(a => a.Description)
            .IsRequired();
        
        builder.Property(a => a.UpdateAt)
            .IsRequired();
        
        builder.Property(a => a.CreatedAt)
            .IsRequired();
        
        builder
            .HasMany(mr => mr.MedicalRecordMedications)
            .WithOne(mrm => mrm.MedicalRecord)
            .HasForeignKey(mrm => mrm.MedicalRecordId);
        
        builder.HasOne(mr => mr.Appointment)
            .WithMany(a => a.MedicalRecords)
            .HasForeignKey(mr => mr.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}