using CourseWorkDataBase.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseWorkDataBase.DAL.Configurations;

public class DoctorDTOConfiguration : IEntityTypeConfiguration<DoctorDTO>
{
    public void Configure(EntityTypeBuilder<DoctorDTO> builder)
    {
        builder.HasNoKey();

        builder.ToView(null);
    }
}