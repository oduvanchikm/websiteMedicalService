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
            .HasForeignKey<Doctor>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.Specialty)
            .WithMany(x => x.Doctors)
            .HasForeignKey(d => d.SpecialtyID)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(x => x.AppointmentSlots)
            .WithOne(x => x.Doctor)
            .HasForeignKey(x => x.DoctorId);
    }
}