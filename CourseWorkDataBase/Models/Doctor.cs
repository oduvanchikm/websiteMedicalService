using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseWorkDataBase.Models;

public class Doctor
{
    public long ID { get; set; }

    [Required]
    [MaxLength(50)]
    public string PersonalNumber { get; set; }

    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; }

    [Required]
    [MaxLength(50)]
    public string FamilyName { get; set; }

    [ForeignKey("Specialty")]
    public long SpecialtyID { get; set; }

    public Specialty Specialty { get; set; }

}