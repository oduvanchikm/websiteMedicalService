using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CourseWorkDataBase.Models;

public class MedicalRecords
{
    [Key]
    public int Id { get; set; }

    [Required] 
    public string Description { get; set; } = string.Empty;
    
    [Required]
    public string Diagnosis { get; set; } = string.Empty;
    
    [Required] 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [Required]
    public DateTime UpdateAt { get; set; } = DateTime.UtcNow;
    
    [ForeignKey("Appointments")] 
    public int AppointmentID { get; set; }
    public Appointments Appointments { get; set; } = null!;
}