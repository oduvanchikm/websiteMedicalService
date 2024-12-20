using CourseWorkDataBase.Models;

namespace CourseWorkDataBase.ViewModels;

public class MedicalRecordWithDoctor
{
    public MedicalRecords medicalRecord { get; set; }
    
    public Doctor doctor { get; set; }
    
    public Clinic clinic { get; set; }
    
    public Specialty specialty { get; set; }
}