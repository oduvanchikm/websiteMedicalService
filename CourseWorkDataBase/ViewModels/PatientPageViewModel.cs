using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using CourseWorkDataBase.Models;

namespace CourseWorkDataBase.ViewModels;

public class PatientPageViewModel
{
    public IEnumerable<DoctorDTO> Doctors { get; set; }

    public IEnumerable<SelectListItem> Specialties { get; set; }

    public long? SelectedSpecialtyId { get; set; }
}
