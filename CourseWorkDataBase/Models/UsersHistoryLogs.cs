namespace CourseWorkDataBase.Models;

public class UsersHistoryLogs
{
    public long HistoryLogsId { get; set; }
    public HistoryLogs HistoryLog { get; set; }
    
    public long UserId { get; set; }
    public User User { get; set; }
}