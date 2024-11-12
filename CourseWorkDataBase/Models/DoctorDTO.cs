namespace CourseWorkDataBase.Models;

public class DoctorDTO
{
    public long ID { get; set; }
    public string FirstName { get; set; }
    public string FamilyName { get; set; }
    public long SpecialtyId { get; set; }
    public string NameSpecialty { get; set; }
}