using CourseWorkDataBase.Models;

namespace CourseWorkDataBase.ViewModels;

public class MedicalHistoryViewModel
{
    public long PatientId { get; set; }
    
    public string PatientName { get; set; }
    
    public string PatientSurname { get; set; }
    
    public List<AddMedicalRecordsViewModel> MedicalRecords { get; set; }
}