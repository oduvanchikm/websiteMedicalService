using CourseWorkDataBase.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseWorkDataBase.DAL.Configurations;

public class SpecialtyConfiguration : IEntityTypeConfiguration<Specialty>
{
    public void Configure(EntityTypeBuilder<Specialty> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.NameSpecialty)
            .IsRequired();

        builder.Property(x => x.Description);
        
        builder.HasOne(x => x.Clinic)
            .WithOne(x => x.Specialty)
            .HasForeignKey<Specialty>(x => x.ClinicId);
    }
}