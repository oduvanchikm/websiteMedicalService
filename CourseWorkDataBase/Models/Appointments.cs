using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseWorkDataBase.Models;

public class Appointments
{
    [Key] 
    public long Id { get; set; }

    [ForeignKey("Patient")] 
    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    
    [ForeignKey("Doctor")] 
    public int DoctorId { get; set; }
    public Doctor Doctor { get; set; } = null!;

    [Required] 
    public DateTime Date { get; set; } = DateTime.UtcNow;

    [Required] 
    public string Status { get; set; } = string.Empty;
}