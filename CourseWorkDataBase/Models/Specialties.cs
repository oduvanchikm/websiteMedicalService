using System.ComponentModel.DataAnnotations;

namespace CourseWorkDataBase.Models;

public class Specialties
{
    [Key] 
    public int Id { get; set; }

    [Required] 
    [StringLength(100)]
    public string NameSpecialty { get; set; } = String.Empty;

    public string? Description { get; set; } = String.Empty;

    public virtual ICollection<Doctors> Doctors { get; set; } = new HashSet<Doctors>();
}