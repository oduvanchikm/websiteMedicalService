using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CourseWorkDataBase.Models;

namespace CourseWorkDataBase.DAL.Configurations;

public class MedicalRecordMedicationConfiguration : IEntityTypeConfiguration<MedicalRecordMedication>
{
    public void Configure(EntityTypeBuilder<MedicalRecordMedication> builder)
    {
        builder.HasKey(mrm => new { mrm.MedicalRecordId, mrm.MedicationId });

        builder
            .HasOne(mrm => mrm.MedicalRecord)
            .WithMany(mr => mr.MedicalRecordMedications)
            .HasForeignKey(mrm => mrm.MedicalRecordId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(mrm => mrm.Medication)
            .WithMany(m => m.MedicalRecordMedications)
            .HasForeignKey(mrm => mrm.MedicationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}