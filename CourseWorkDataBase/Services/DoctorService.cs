using CourseWorkDataBase.Models;
using CourseWorkDataBase.ViewModels;
using CourseWorkDataBase.DAL;
using Microsoft.EntityFrameworkCore;

namespace CourseWorkDataBase.Services;

public class DoctorService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DoctorService> _logger;

    public DoctorService(ApplicationDbContext context, ILogger<DoctorService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<MedicalRecords> AddMedicalRecordsAsync(
        long? appointmentId,
        string description,
        string diagnosis,
        long? medicationId,
        string? nameMedication,
        string? descriptionMedication)
    {
        if (medicationId != null)
        {
            Console.Out.WriteLine(medicationId);
        }
        if (nameMedication != null && descriptionMedication != null)
        {
            Console.Out.WriteLine(descriptionMedication);
            Console.Out.WriteLine(nameMedication);
        }
        
        using (var transaction = _context.Database.BeginTransaction())
        {
            try
            {
                Medications medication;
                if (medicationId.HasValue)
                {
                    medication = await _context.Medications
                        .FindAsync(medicationId.Value);
                    if (medication == null)
                    {
                        Console.Out.WriteLine("MEDICATION  not found.!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        throw new ApplicationException("Medication not found.");
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(nameMedication) && string.IsNullOrEmpty(descriptionMedication))
                    {
                        throw new ApplicationException("The clinic address is required.");
                    }
                    
                    medication = await _context.Medications
                        .FirstOrDefaultAsync(s => s.Name == nameMedication);
                    if (medication != null)
                    {
                        throw new ApplicationException("A medication with this name already exists.");
                    }

                    medication = new Medications
                    {
                        Name = nameMedication,
                        Description = descriptionMedication
                    };
                    
                    Console.Out.WriteLine("A new medication has created with ID: " + medication.MedicationId);
                
                    _context.Medications.Add(medication);
                    await _context.SaveChangesAsync();
                    Console.Out.WriteLine("A new medication has created with ID: " + medication.MedicationId);
                }

                var appointment = await _context.Appointments
                    .Include(a => a.MedicalRecords)
                    .FirstOrDefaultAsync(a => a.Id == appointmentId);
                if (appointment == null)
                {
                    throw new ApplicationException("Appointment not found.");
                }

                MedicalRecords medicalRecords = new MedicalRecords()
                {
                    Description = description,
                    Diagnosis = diagnosis,
                    CreatedAt = DateTime.UtcNow,
                    UpdateAt = DateTime.UtcNow,
                    AppointmentId = appointment.Id
                };
                
                _context.MedicalRecords.Add(medicalRecords);
                await _context.SaveChangesAsync();
                
                var medicalRecordMedication = new MedicalRecordMedication
                {
                    MedicalRecordId = medicalRecords.Id,
                    MedicationId = medication.MedicationId
                };
                
                _context.MedicalRecordMedications.Add(medicalRecordMedication);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return medicalRecords;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}