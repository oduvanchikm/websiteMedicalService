using Microsoft.AspNetCore.Mvc.Rendering;

namespace CourseWorkDataBase.Models;

public class MedicalRecords
{
    public long Id { get; set; }
    
    public string Description { get; set; }
    
    public string Diagnosis { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdateAt { get; set; }
    
    public Appointment? Appointments { get; set; }
    
    public IEnumerable<MedicalRecordMedication> MedicalRecordMedications { get; set; }
}