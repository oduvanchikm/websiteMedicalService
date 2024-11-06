using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseWorkDataBase.Models;

public class AddDoctorRequest
{
    [Required]
    public string Email { get; set; }

    [Required]
    public string FirstName { get; set; }
    [Required]
    public string FamilyName { get; set; }
    
    public long? SpecialtyId { get; set; }

    [Required]
    public string ClinicAddress { get; set; } 
    [Required]
    public string ClinicPhoneNumber { get; set; } 
}
