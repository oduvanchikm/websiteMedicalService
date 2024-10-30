using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseWorkDataBase.Models;

public class Doctors
{
    [Key] 
    public int Id { get; set; }

    [Required] 
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required] 
    [MaxLength(100)]
    public string FamilyName { get; set; } = string.Empty;

    [ForeignKey("Specialties")]
    public int SpecialtyID { get; set; }
    public virtual Specialties Specialty { get; set; } = null!;
    
    public virtual ICollection<Appointments> Appointments { get; set; } = new HashSet<Appointments>();
}