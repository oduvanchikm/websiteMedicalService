using CourseWorkDataBase.Models;

namespace CourseWorkDataBase.ViewModels;

public class PdfData
{
    public string FullPath { get; set; }
    public Patient Patient { get; set; }
    
    public List<MedicalRecordWithDoctor> medicalRecordWithDoctor { get; set; }
}