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
        
        builder.Property(x => x.CreatedAt)
            .IsRequired();
        
        builder.HasOne(x => x.Role)
            .WithMany(r => r.User)
            .HasForeignKey(x => x.RoleId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(x => x.Patient)
            .WithOne(p => p.User)
            .HasForeignKey<Patient>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.Doctor)
            .WithOne(d => d.User)
            .HasForeignKey<Doctor>(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder
            .HasMany(u => u.UsersHistoryLogsEnumerable)
            .WithOne(u => u.User)
            .HasForeignKey(u => u.UserId);
        
        var fixedCreatedAt = new DateTime(2024, 10, 1, 0, 0, 0, DateTimeKind.Utc);
        
        builder.HasData(
            new User() 
            { 
                Id = 1, 
                Email = "admin@gmail.com",
                Password = "$2a$11$o.sTnyjh8Mr9ArOWpr5Q..rsRPFHJ7EJ6pIeFUyVEfP2fe5b1riHm", 
                RoleId = 1, 
                CreatedAt = fixedCreatedAt,
                Patient = null,
                Doctor = null
            }
        );

    }
}