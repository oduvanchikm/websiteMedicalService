using CourseWorkDataBase.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseWorkDataBase.DAL.Configurations;

public class UsersHistoryLogsConfiguration : IEntityTypeConfiguration<UsersHistoryLogs>
{
    public void Configure(EntityTypeBuilder<UsersHistoryLogs> builder)
    {
        builder.HasKey(uhl => new { uhl.UserId, uhl.HistoryLogsId });

        builder
            .HasOne(mrm => mrm.User)
            .WithMany(mr => mr.UsersHistoryLogsEnumerable)
            .HasForeignKey(mrm => mrm.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(mrm => mrm.HistoryLog)
            .WithMany(m => m.UsersHistoryLogsEnumerable)
            .HasForeignKey(mrm => mrm.HistoryLogsId) 
            .OnDelete(DeleteBehavior.Cascade);
    }
}