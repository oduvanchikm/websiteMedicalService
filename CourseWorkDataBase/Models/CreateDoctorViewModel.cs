using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CourseWorkDataBase.Models;

public class CreateDoctorViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "Пароль должен содержать не менее 6 символов.")]
    public string Password { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
    public string ConfirmPassword { get; set; }

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; }

    [Required]
    [MaxLength(100)]
    public string FamilyName { get; set; }

    public string ContactInfo { get; set; }

    [Required]
    public string SpecialtyId { get; set; }

    public List<Specialty> Specialties { get; set; }
}