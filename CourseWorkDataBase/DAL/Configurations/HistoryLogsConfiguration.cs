using CourseWorkDataBase.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseWorkDataBase.DAL.Configurations;

public class HistoryLogsConfiguration : IEntityTypeConfiguration<HistoryLogs>
{
    public void Configure(EntityTypeBuilder<HistoryLogs> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.OperationType)
            .IsRequired();

        builder.Property(x => x.TableName)
            .IsRequired();

        builder.Property(x => x.ChangeTime)
            .IsRequired();
        
        builder
            .HasMany(hl => hl.UsersHistoryLogsEnumerable)
            .WithOne(hl => hl.HistoryLog)
            .HasForeignKey(hl => hl.HistoryLogsId);
    }
}