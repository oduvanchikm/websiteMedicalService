using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using CourseWorkDataBase.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Rendering;
using CourseWorkDataBase.Services;
using CourseWorkDataBase.ViewModels;
using CourseWorkDataBase.Helpers;
using Microsoft.EntityFrameworkCore;

namespace CourseWorkDataBase.Controllers;

[Authorize("DoctorPolicy")]
public class DoctorController(
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    ILogger<DoctorController> logger,
    DoctorService doctorService,
    IConfiguration configuration)
    : Controller
{
    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("AuthorizationPage", "Authorization");
    }

    private long GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && long.TryParse(userIdClaim.Value, out long userId))
        {
            return userId;
        }

        throw new InvalidOperationException("Couldn't get the ID of the current user.");
    }

    public async Task<IActionResult> DoctorPage()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            logger.LogWarning("User is not logged in.");
            return NotFound();
        }

        await using var context = await dbContextFactory.CreateDbContextAsync();

        var doctor = await context.Doctors
            .Where(d => d.UserId == userId)
            .FirstOrDefaultAsync();
        if (doctor == null)
        {
            logger.LogWarning("Doctor is not logged in.");
            return NotFound();
        }

        var appointmentSlots = await context.AppointmentSlots
            .Include(a => a.Appointment)
            .ThenInclude(ap => ap.Patient)
            .Where(a => a.DoctorId == doctor.ID && a.IsBooked == true)
            .ToListAsync();
        return View(appointmentSlots);
    }

    private async Task<IEnumerable<SelectListItem>> GetMedicationsSelectListAsync()
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();

        var medications = await context.Medications
            .OrderBy(x => x.Name)
            .ToListAsync();

        return medications.Select(s => new SelectListItem
        {
            Value = s.MedicationId.ToString(),
            Text = s.Name
        }).ToList();
    }

    [HttpGet]
    public async Task<IActionResult> AddMedicalRecords(long id)
    {
        if (id <= 0)
        {
            logger.LogWarning("Invalid appointment ID.");
            return NotFound();
        }

        var medications = await GetMedicationsSelectListAsync();

        var model = new AddMedicalRecordsViewModel
        {
            MedicationsList = medications,
            AppointmentId = id
        };

        Console.Out.WriteLine($"in get : {model.AppointmentId}");

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddMedicalRecords(AddMedicalRecordsViewModel model)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();

        if (!ModelState.IsValid)
        {
            var medicationsList = await context.Medications.ToListAsync();
            model.MedicationsList = medicationsList.Select(m => new SelectListItem
            {
                Value = m.MedicationId.ToString(),
                Text = m.Name
            });
            return View(model);
        }

        try
        {
            var medicalRecords = await doctorService.AddMedicalRecordsAsync(
                model.AppointmentId,
                model.Description,
                model.Diagnosis,
                model.MedicationID,
                model.NameMedication,
                model.DescriptionMedication);

            return RedirectToAction("DoctorPage", "Doctor");
        }
        catch (Exception ex)
        {
            if (ex.InnerException != null)
            {
                Console.Out.WriteLine($"Internal error: {ex.InnerException.Message}");
            }

            ModelState.AddModelError("", "An unknown error has occurred. Please try again later.");
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> ShowMedicalRecords(long id) // это айдишником пока что бцдет пациенат
    {
        if (id <= 0)
        {
            logger.LogWarning("Invalid patient ID in show medical records.");
            return NotFound();
        }

        await using var context = await dbContextFactory.CreateDbContextAsync();

        var patient = await context.Patients
            .AsNoTracking()
            .Include(p => p.Appointments)
            .ThenInclude(appt => appt.MedicalRecords)
            .ThenInclude(mr => mr.MedicalRecordMedications)
            .ThenInclude(mrm => mrm.Medication)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (patient == null)
        {
            logger.LogWarning($"Patient with ID {id} not found.");
            return NotFound();
        }

        var medicalRecordsViewModel = patient.Appointments
            .SelectMany(appt => appt.MedicalRecords, (appt, mr) => new { appt, mr })
            .SelectMany(record => record.mr.MedicalRecordMedications, (record, mrm) => new AddMedicalRecordsViewModel
            {
                Description = record.mr.Description,
                Diagnosis = record.mr.Diagnosis,
                NameMedication = mrm.Medication?.Name ?? "not found",
                DescriptionMedication = mrm.Medication?.Description ?? "not found",
            })
            .ToList();

        var viewModel = new MedicalHistoryViewModel
        {
            PatientId = patient.Id,
            PatientName = patient.FirstName,
            PatientSurname = patient.FamilyName,
            MedicalRecords = medicalRecordsViewModel
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult>  CreatePdfFileWithMedicalRecordsDoctor(long id)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();

        Console.Out.WriteLine($"in get : {id}");
        
        var patient = await context.Patients
            .AsNoTracking()
            .Include(p => p.Appointments)
            .ThenInclude(appt => appt.AppointmentSlot)
            .ThenInclude(slot => slot.Doctor).ThenInclude(doctor => doctor.Clinic)
            .Include(patient => patient.Appointments).ThenInclude(appointment => appointment.AppointmentSlot)
            .ThenInclude(appointmentSlot => appointmentSlot.Doctor).ThenInclude(doctor => doctor.Specialty)
            .Include(p => p.Appointments)
            .ThenInclude(appt => appt.MedicalRecords)
            .ThenInclude(mr => mr.MedicalRecordMedications)
            .ThenInclude(mrm => mrm.Medication)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (patient == null)
        {
            throw new Exception($"Patient with ID {patient.Id} not found.");
        }

        var appointments = patient.Appointments?.ToList();
        if (appointments == null || !appointments.Any())
        {
            throw new Exception($"No appointments found for patient with ID {id}.");
        }

        var medicalRecordsWithDoctors = appointments
            .Select(appt => new
            {
                Appointment = appt,
                Doctor = appt.AppointmentSlot.Doctor,
                Clinic = appt.AppointmentSlot.Doctor?.Clinic,
                Specialty = appt.AppointmentSlot.Doctor?.Specialty
            })
            .Where(x => x.Doctor != null) 
            .SelectMany(x => x.Appointment.MedicalRecords.Select(mr => new MedicalRecordWithDoctor
            {
                medicalRecord = mr,
                doctor = x.Doctor,
                clinic = x.Clinic,
                specialty = x.Specialty
            }))
            .ToList();

        if (!medicalRecordsWithDoctors.Any())
        {
            throw new Exception($"Not medical record with ID {id}.");
        }

        foreach (var mrwd in medicalRecordsWithDoctors)
        {
            Console.Out.WriteLine($"ID medical record: {mrwd.medicalRecord.Id}, Doctor ID: {mrwd.doctor.ID}");
        }

        var fileName = $"MedicalRecords_{patient.FamilyName}_{patient.FirstName}_{id}.pdf";
        var backupFolder = configuration["PdfFileConfig:PdfFolderPath"];
        if (!Directory.Exists(backupFolder))
        {
            Directory.CreateDirectory(backupFolder);
        }
        
        var fullPath = Path.Combine(backupFolder, fileName);

        var pdfData = new PdfData
        {
            FullPath = fullPath,
            Patient = patient,
            medicalRecordWithDoctor = medicalRecordsWithDoctors
        };

        try
        {
            await SavePdfFile.CreatePdfFileWithMedicalRecords(pdfData);
        }
        catch (IOException ex) 
        {
            throw new Exception($"PdfException occurred: {ex.Message}");
        }
        catch (Exception ex)
        {
            throw new Exception($"Unexpected exception occurred: {ex.Message}");
        }

        var fileBytes = System.IO.File.ReadAllBytes(fullPath);

        if (!System.IO.File.Exists(fullPath))
        {
            throw new FileNotFoundException($"File not found at {fullPath}");
        }

        return File(fileBytes, "application/pdf", fileName);
    }
}