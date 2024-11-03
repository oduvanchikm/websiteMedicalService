using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseWorkDataBase.Models;

public class Appointment
{
    [Key] 
    public long Id { get; set; }

    [ForeignKey("Patient")] 
    public long PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    
    [ForeignKey("AppointmentSlot")]
    public long AppointmentSlotId { get; set; }
    public AppointmentSlot AppointmentSlot { get; set; }

    [Required] 
    public DateTime Date { get; set; } = DateTime.UtcNow;

    [Required] 
    public string Status { get; set; } = string.Empty;
}