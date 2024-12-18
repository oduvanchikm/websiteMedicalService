using CourseWorkDataBase.Models;
using CourseWorkDataBase.ViewModels;
using CourseWorkDataBase.DAL;
using Microsoft.EntityFrameworkCore;

namespace CourseWorkDataBase.Services;

public class DoctorService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    private readonly ILogger<DoctorService> _logger;

    public DoctorService(IDbContextFactory<ApplicationDbContext> dbContextFactory, ILogger<DoctorService> logger)
    {
        _dbContextFactory = dbContextFactory;
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
        
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        
        using (var transaction = context.Database.BeginTransaction())
        {
            try
            {
                Medications medication;
                if (medicationId.HasValue)
                {
                    medication = await context.Medications
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
                    
                    medication = await context.Medications
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
                
                    context.Medications.Add(medication);
                    await context.SaveChangesAsync();
                    Console.Out.WriteLine("A new medication has created with ID: " + medication.MedicationId);
                }

                var appointment = await context.Appointments
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
                
                context.MedicalRecords.Add(medicalRecords);
                await context.SaveChangesAsync();
                
                var medicalRecordMedication = new MedicalRecordMedication
                {
                    MedicalRecordId = medicalRecords.Id,
                    MedicationId = medication.MedicationId
                };
                
                context.MedicalRecordMedications.Add(medicalRecordMedication);
                await context.SaveChangesAsync();

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