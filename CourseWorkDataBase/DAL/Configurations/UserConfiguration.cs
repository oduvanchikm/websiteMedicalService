using CourseWorkDataBase.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseWorkDataBase.DAL.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(x => x.Password)
            .IsRequired();

        builder.Property(x => x.PersonalNumber)
            .IsRequired(false);
        
        builder.Property(x => x.CreatedAt)
            .IsRequired();
        
        builder.HasOne(x => x.Role)
            .WithMany(r => r.User)
            .HasForeignKey(x => x.RoleId)
            .IsRequired();
        
        builder.HasOne(x => x.Patient)
            .WithOne(p => p.User)
            .HasForeignKey<Patient>(p => p.UserId);
        
        builder.HasOne(x => x.Doctor)
            .WithOne(d => d.User)
            .HasForeignKey<Doctor>(d => d.UserId);
    }
}