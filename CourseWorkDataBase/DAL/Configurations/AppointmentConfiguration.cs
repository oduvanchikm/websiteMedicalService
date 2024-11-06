using CourseWorkDataBase.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseWorkDataBase.DAL.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Date)
            .IsRequired();
        
        builder.HasOne(x => x.AppointmentSlot)
            .WithOne(x => x.Appointment)
            .HasForeignKey<Appointment>(x => x.AppointmentSlotId);
        
        builder.HasOne(x => x.Status)
            .WithMany(r => r.Appointments)
            .HasForeignKey(x => x.StatusId)
            .IsRequired();
    }
}