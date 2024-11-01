using System.ComponentModel.DataAnnotations;

namespace CourseWorkDataBase.Models;

public class Specialty
{
    [Key] 
    public long Id { get; set; }

    [Required] 
    [StringLength(100)]
    public string NameSpecialty { get; set; } 

    public string? Description { get; set; } 
    
    public long? ClinicId { get; set; }
}