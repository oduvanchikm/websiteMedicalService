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
    }
}