using Microsoft.AspNetCore.Mvc.Rendering;

namespace CourseWorkDataBase.Models;

public class AddDoctorViewModel
{
    public AddDoctorRequest Doctor { get; set; }
    public IEnumerable<SelectListItem> Specialties { get; set; }
}