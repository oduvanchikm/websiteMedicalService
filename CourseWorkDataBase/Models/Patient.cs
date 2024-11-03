using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseWorkDataBase.Models;

public class Patient
{
    public long Id { get; set; }

    [ForeignKey("ApplicationUser")]
    public long UserID { get; set; }
    public User User { get; set; }

    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; }

    [Required]
    [MaxLength(50)]
    public string FamilyName { get; set; }

    public string Gender { get; set; }
}