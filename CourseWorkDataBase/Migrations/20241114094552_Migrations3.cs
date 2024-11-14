using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseWorkDataBase.Migrations
{
    /// <inheritdoc />
    public partial class Migrations3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicalRecordMedication_MedicalRecords_MedicalRecordId",
                table: "MedicalRecordMedication");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicalRecordMedication_Medications_MedicationId",
                table: "MedicalRecordMedication");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MedicalRecordMedication",
                table: "MedicalRecordMedication");

            migrationBuilder.RenameTable(
                name: "MedicalRecordMedication",
                newName: "MedicalRecordMedications");

            migrationBuilder.RenameIndex(
                name: "IX_MedicalRecordMedication_MedicationId",
                table: "MedicalRecordMedications",
                newName: "IX_MedicalRecordMedications_MedicationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MedicalRecordMedications",
                table: "MedicalRecordMedications",
                columns: new[] { "MedicalRecordId", "MedicationId" });

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalRecordMedications_MedicalRecords_MedicalRecordId",
                table: "MedicalRecordMedications",
                column: "MedicalRecordId",
                principalTable: "MedicalRecords",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalRecordMedications_Medications_MedicationId",
                table: "MedicalRecordMedications",
                column: "MedicationId",
                principalTable: "Medications",
                principalColumn: "MedicationId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicalRecordMedications_MedicalRecords_MedicalRecordId",
                table: "MedicalRecordMedications");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicalRecordMedications_Medications_MedicationId",
                table: "MedicalRecordMedications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MedicalRecordMedications",
                table: "MedicalRecordMedications");

            migrationBuilder.RenameTable(
                name: "MedicalRecordMedications",
                newName: "MedicalRecordMedication");

            migrationBuilder.RenameIndex(
                name: "IX_MedicalRecordMedications_MedicationId",
                table: "MedicalRecordMedication",
                newName: "IX_MedicalRecordMedication_MedicationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MedicalRecordMedication",
                table: "MedicalRecordMedication",
                columns: new[] { "MedicalRecordId", "MedicationId" });

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalRecordMedication_MedicalRecords_MedicalRecordId",
                table: "MedicalRecordMedication",
                column: "MedicalRecordId",
                principalTable: "MedicalRecords",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalRecordMedication_Medications_MedicationId",
                table: "MedicalRecordMedication",
                column: "MedicationId",
                principalTable: "Medications",
                principalColumn: "MedicationId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
