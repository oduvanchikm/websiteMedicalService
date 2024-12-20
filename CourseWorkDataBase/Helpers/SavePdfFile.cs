using System.Globalization;
using CourseWorkDataBase.DAL;
using CourseWorkDataBase.Models;
using CourseWorkDataBase.ViewModels;
using Microsoft.EntityFrameworkCore;
using iText.IO.Font;
using iText.Kernel.Exceptions;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Element;
using Microsoft.AspNetCore.Mvc;

namespace CourseWorkDataBase.Helpers;

public class SavePdfFile
{
    private readonly ILogger<SavePdfFile> _logger;

    public SavePdfFile(ILogger<SavePdfFile> logger)
    {
        _logger = logger;
    }

    public static async Task CreatePdfFileWithMedicalRecords(PdfData pdfData)
    {
        var directory = Path.GetDirectoryName(pdfData.FullPath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using (var writer = new PdfWriter(pdfData.FullPath))
        using (var pdf = new PdfDocument(writer))
        {
            using (var document = new iText.Layout.Document(pdf))
            {
                var fontPath = "/System/Library/Fonts/Supplemental/Arial.ttf";
                if (!File.Exists(fontPath))
                {
                    throw new FileNotFoundException($"Файл шрифта не найден: {fontPath}");
                }

                var font = PdfFontFactory.CreateFont(fontPath, PdfEncodings.IDENTITY_H);
                document.SetFont(font);

                var nameAndSurname = $"{pdfData.Patient.FirstName} {pdfData.Patient.FamilyName}";
                document.Add(new Paragraph("Medical Records").SetFontSize(20));
                document.Add(new Paragraph($"Patient's Name: {nameAndSurname}").SetFontSize(14));
                document.Add(new Paragraph("\nRecords:").SetFontSize(16));

                var table = new Table(6);
                table.AddHeaderCell("Date");
                table.AddHeaderCell("Doctor Name");
                table.AddHeaderCell("Clinic Address");
                table.AddHeaderCell("Specialty");
                table.AddHeaderCell("Diagnosis");
                table.AddHeaderCell("Description");

                foreach (var record in pdfData.medicalRecordWithDoctor)
                {
                    var doctorName = $"{record.doctor.FirstName} {record.doctor.FamilyName}";
                    var clinicAddress = record.clinic?.Address ?? "N/A";
                    var specialtyName = record.specialty?.NameSpecialty ?? "N/A";
                    
                    var createdAt = record.medicalRecord.CreatedAt.ToString(DateTimeFormatInfo.InvariantInfo);
                    var diagnosis = record.medicalRecord.Diagnosis ?? "N/A";
                    var description = record.medicalRecord.Description ?? "N/A";

                    table.AddCell(new Cell().Add(new Paragraph(createdAt)).SetFontSize(14));
                    table.AddCell(new Cell().Add(new Paragraph(doctorName)).SetFontSize(14));
                    table.AddCell(new Cell().Add(new Paragraph(clinicAddress)).SetFontSize(14));
                    table.AddCell(new Cell().Add(new Paragraph(specialtyName)).SetFontSize(14));
                    table.AddCell(new Cell().Add(new Paragraph(diagnosis)).SetFontSize(14));
                    table.AddCell(new Cell().Add(new Paragraph(description)).SetFontSize(14));
                }

                document.Add(table);
            }
        }
    }
}