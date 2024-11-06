using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseWorkDataBase.Models;

public class AppointmentSlot
{
    public long Id { get; set; }
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset EndTime { get; set; }
    
    public Doctor Doctor { get; set; }
    public long DoctorId { get; set; }
    
    public Appointment Appointment { get; set; }
    public bool IsBooked { get; set; }
}