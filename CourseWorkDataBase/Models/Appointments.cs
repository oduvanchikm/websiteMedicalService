using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseWorkDataBase.Models;

public class Appointments
{
    [Key] 
    public int AppointmentId { get; set; }

    [ForeignKey("Users")] 
    public int UserId { get; set; }
    public Users User { get; set; } = null!;

    
    [ForeignKey("Doctors")] 
    public int DoctorId { get; set; }
    public Doctors Doctor { get; set; } = null!;

    [Required] 
    public DateTime Date { get; set; } = DateTime.UtcNow;

    [Required] 
    public string Status { get; set; } = string.Empty;
    
    // todo add reference to Clinic??????
}