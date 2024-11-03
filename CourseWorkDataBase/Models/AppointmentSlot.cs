using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseWorkDataBase.Models;

public class AppointmentSlot
{
    public long Id { get; set; }
    
    [Required]
    public DateTimeOffset StartTime { get; set; }

    [Required]
    public DateTimeOffset EndTime { get; set; }
    
    [ForeignKey("Doctor")]
    public long DoctorId { get; set; }
    public Doctor Doctor { get; set; }
    
    public Appointment Appointment { get; set; }
    [NotMapped]
    public bool IsBooked => Appointment != null;
}