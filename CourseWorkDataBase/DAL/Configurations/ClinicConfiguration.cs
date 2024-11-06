using CourseWorkDataBase.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseWorkDataBase.DAL.Configurations;

public class ClinicConfiguration : IEntityTypeConfiguration<Clinic>
{
    public void Configure(EntityTypeBuilder<Clinic> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Address)
            .IsRequired();

        builder.Property(x => x.PhoneNumber)
            .IsRequired();
    }
}