namespace CourseWorkDataBase.Models;

public class AppointmentViewModel
{
    public int Id { get; set; } 
    public DateTime AppointmentDate { get; set; }
    public string PatientName { get; set; } = string.Empty;

}