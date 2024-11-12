using CourseWorkDataBase.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseWorkDataBase.DAL.Configurations;

public class StatusConfiguration : IEntityTypeConfiguration<Status>
{
    public void Configure(EntityTypeBuilder<Status> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Name)
            .IsRequired();
        
        builder.HasMany(x => x.Appointments)
            .WithOne(u => u.Status)
            .HasForeignKey(u => u.StatusId);
        
        builder.HasData(
            new Status() { Id = 1, Name = "Booked"},
            new Status() { Id = 2, Name = "Not booked"},
            new Status() { Id = 3, Name = "Canceled"}
        );
    }
}