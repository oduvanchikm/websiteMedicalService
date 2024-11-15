using Microsoft.AspNetCore.Mvc.Rendering;

namespace CourseWorkDataBase.ViewModels;

public class AddMedicalRecordsViewModel
{
    public long AppointmentId { get; set; }

    public string Description { get; set; }

    public string Diagnosis { get; set; }
    
    public long? MedicationID { get; set; }
    
    public string? NameMedication { get; set; }
    
    public string? DescriptionMedication { get; set; } 
    
    public IEnumerable<SelectListItem>? MedicationsList { get; set; }
}