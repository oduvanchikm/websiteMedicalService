using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseWorkDataBase.ViewModels;

public class AddDoctorRequest
{
    [Required]
    [EmailAddress(ErrorMessage = "Некорректный формат Email")]
    public string email { get; set; }

    [Required]
    public string firstName { get; set; }

    [Required]
    public string familyName { get; set; }
    
    [Required]
    public string personalNumber { get; set; }
 
    public long? specialtyId { get; set; }
    
    public string? specialtyName { get; set; }
    
    public string? description { get; set; }
    
    public IEnumerable<SelectListItem>? Specialties { get; set; }
    
    public long? clinicId { get; set; }
    
    public string? clinicAddress { get; set; } 
    public string? clinicPhoneNumber { get; set; } 
    
    public IEnumerable<SelectListItem>? Clinics { get; set; }
}
