using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseWorkDataBase.Models;

public class AddDoctorRequest
{
    [Required(ErrorMessage = "Email обязателен")]
    [EmailAddress(ErrorMessage = "Некорректный формат Email")]
    public string email { get; set; }

    [Required(ErrorMessage = "FirstName обязателен")]
    public string firstName { get; set; }

    [Required(ErrorMessage = "FamilyName обязателен")]
    public string familyName { get; set; }
 
    // public long? SpecialtyId { get; set; }
    // [Required]
    // public string ClinicAddress { get; set; } 
    // [Required]
    // public string ClinicPhoneNumber { get; set; } 
}
