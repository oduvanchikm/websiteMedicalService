namespace CourseWorkDataBase.Models;

public class MedicalRecordMedication
{
    public long MedicalRecordId { get; set; }
    public MedicalRecords MedicalRecord { get; set; }

    public long MedicationId { get; set; }
    public Medications Medication { get; set; }
}
