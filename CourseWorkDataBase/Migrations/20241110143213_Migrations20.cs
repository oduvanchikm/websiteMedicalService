using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseWorkDataBase.Migrations
{
    /// <inheritdoc />
    public partial class Migrations20 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Specialties_Clinics_ClinicId",
                table: "Specialties");

            migrationBuilder.DropIndex(
                name: "IX_Specialties_ClinicId",
                table: "Specialties");

            migrationBuilder.DropColumn(
                name: "ClinicId",
                table: "Specialties");

            migrationBuilder.AddColumn<long>(
                name: "ClinicId",
                table: "Doctors",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_ClinicId",
                table: "Doctors",
                column: "ClinicId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_Clinics_ClinicId",
                table: "Doctors",
                column: "ClinicId",
                principalTable: "Clinics",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_Clinics_ClinicId",
                table: "Doctors");

            migrationBuilder.DropIndex(
                name: "IX_Doctors_ClinicId",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "ClinicId",
                table: "Doctors");

            migrationBuilder.AddColumn<long>(
                name: "ClinicId",
                table: "Specialties",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Specialties_ClinicId",
                table: "Specialties",
                column: "ClinicId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Specialties_Clinics_ClinicId",
                table: "Specialties",
                column: "ClinicId",
                principalTable: "Clinics",
                principalColumn: "Id");
        }
    }
}
