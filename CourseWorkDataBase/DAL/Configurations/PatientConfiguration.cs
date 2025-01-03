using CourseWorkDataBase.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseWorkDataBase.DAL.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FirstName)
            .IsRequired();
        
        builder.Property(x => x.FamilyName)
            .IsRequired();

        builder.Property(x => x.Gender)
            .IsRequired();
        
        builder.Property(x => x.Date)
            .IsRequired();
        
        builder.HasOne(x => x.User)
            .WithOne(x => x.Patient)
            .HasForeignKey<Patient>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}