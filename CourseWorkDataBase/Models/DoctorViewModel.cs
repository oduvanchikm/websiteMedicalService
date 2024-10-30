namespace CourseWorkDataBase.Models;

public class DoctorViewModel
{
    public int DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string DoctorFamilyName { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
    public List<AppointmentViewModel> Appointments { get; set; } = new List<AppointmentViewModel>();

}