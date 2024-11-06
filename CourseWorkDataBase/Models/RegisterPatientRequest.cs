using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseWorkDataBase.Models;

public class RegisterPatientRequest
{
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }

    [Required]
    public string FirstName { get; set; }

    [Required]
    public string FamilyName { get; set; }

    public string Gender { get; set; }

}