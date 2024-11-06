using CourseWorkDataBase.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseWorkDataBase.DAL.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Name)
            .IsRequired();
        
        builder.HasMany(x => x.User)
            .WithOne(u => u.Role)
            .HasForeignKey(u => u.RoleId)
            .IsRequired();
        
        builder.HasData(
            new Role() { Id = 1, Name = "Admin"},
            new Role() { Id = 2, Name = "Doctor"},
            new Role() { Id = 3, Name = "Patient"}
        );
    }
}