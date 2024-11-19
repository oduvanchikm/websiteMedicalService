namespace CourseWorkDataBase.Models;

public class HistoryLogs
{
    public long Id { get; set; }
    
    public string TableName { get; set; }
    
    public string OperationType { get; set; }
    
    public DateTime ChangeTime { get; set; }
    
    public IEnumerable<UsersHistoryLogs> UsersHistoryLogsEnumerable { get; set; }
}