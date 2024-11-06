using CourseWorkDataBase.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseWorkDataBase.DAL.Configurations;

public class AppointmentSlotConfiguration : IEntityTypeConfiguration<AppointmentSlot>
{
    public void Configure(EntityTypeBuilder<AppointmentSlot> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.StartTime)
            .IsRequired();

        builder.Property(x => x.EndTime)
            .IsRequired();
        
        builder.Property(x => x.IsBooked)
            .IsRequired();
        
        builder.HasOne(x => x.Doctor)
            .WithMany(x => x.AppointmentSlots)
            .HasForeignKey(f => f.DoctorId);
    }
}