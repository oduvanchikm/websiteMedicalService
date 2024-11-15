namespace CourseWorkDataBase.Models;

public class Medications
{
    public long MedicationId { get; set; }
    
    public string Name { get; set; }
    
    public string Description { get; set; }

    public ICollection<MedicalRecordMedication> MedicalRecordMedications { get; set; }
}