using System.ComponentModel.DataAnnotations;

namespace CourseWorkDataBase.Models;

public class Specialty
{
    public long Id { get; set; }
    
    public string NameSpecialty { get; set; } 

    public string? Description { get; set; } 

    public List<Doctor> Doctors { get; set; }
}