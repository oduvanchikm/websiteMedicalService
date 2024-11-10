namespace CourseWorkDataBase.ViewModels;

public class CalendarViewModel
{
    public List<MonthSlots> Months { get; set; }
}

public class MonthSlots
{
    public int Year { get; set; } 
    public int Month { get; set; }
    public string MonthName { get; set; }
    public List<DaySlots> Days { get; set; }
}

public class DaySlots
{
    public DateTime Date { get; set; }
    public List<TimeSlotViewModel> Slots { get; set; }
}

public class TimeSlotViewModel
{
    public long Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string DoctorName { get; set; } 
}