using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CourseWorkDataBase.Models;

public class MedicationsConfiguration : IEntityTypeConfiguration<Medications>
{
    public void Configure(EntityTypeBuilder<Medications> builder)
    {
        builder.HasKey(m => m.MedicationId);

        builder.Property(m => m.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(m => m.Description)
            .HasMaxLength(500);

        builder
            .HasMany(m => m.MedicalRecordMedications)
            .WithOne(mrm => mrm.Medication)
            .HasForeignKey(mrm => mrm.MedicationId);
    }
}