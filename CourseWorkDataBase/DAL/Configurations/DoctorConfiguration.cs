using CourseWorkDataBase.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseWorkDataBase.DAL.Configurations;

public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.HasKey(x => x.ID);
        
        builder.Property(x => x.FirstName)
            .IsRequired();
        
        builder.Property(x => x.FamilyName)
            .IsRequired();
        
        builder.HasOne(x => x.User)
            .WithOne(x => x.Doctor)
            .HasForeignKey<Doctor>(x => x.UserId);
        
        builder.HasOne(x => x.Specialty)
            .WithOne(x => x.Doctor)
            .HasForeignKey<Doctor>(x => x.SpecialtyID);
        
        builder.HasMany(x => x.AppointmentSlots)
            .WithOne(x => x.Doctor)
            .HasForeignKey(x => x.DoctorId);
    }
}