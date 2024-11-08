using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseWorkDataBase.Migrations
{
    /// <inheritdoc />
    public partial class Migrations19 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_Specialties_SpecialtyID",
                table: "Doctors");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_Users_UserId",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Doctors_SpecialtyID",
                table: "Doctors");

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_SpecialtyID",
                table: "Doctors",
                column: "SpecialtyID");

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_Specialties_SpecialtyID",
                table: "Doctors",
                column: "SpecialtyID",
                principalTable: "Specialties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Users_UserId",
                table: "Patients",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_Specialties_SpecialtyID",
                table: "Doctors");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_Users_UserId",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Doctors_SpecialtyID",
                table: "Doctors");

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_SpecialtyID",
                table: "Doctors",
                column: "SpecialtyID",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_Specialties_SpecialtyID",
                table: "Doctors",
                column: "SpecialtyID",
                principalTable: "Specialties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Users_UserId",
                table: "Patients",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
