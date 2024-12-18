using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseWorkDataBase.Migrations
{
    /// <inheritdoc />
    public partial class final8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersHistoryLogs_HistoryLogs_HistoryLogsId",
                table: "UsersHistoryLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersHistoryLogs_Users_UserId",
                table: "UsersHistoryLogs");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersHistoryLogs_HistoryLogs_HistoryLogsId",
                table: "UsersHistoryLogs",
                column: "HistoryLogsId",
                principalTable: "HistoryLogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersHistoryLogs_Users_UserId",
                table: "UsersHistoryLogs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersHistoryLogs_HistoryLogs_HistoryLogsId",
                table: "UsersHistoryLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersHistoryLogs_Users_UserId",
                table: "UsersHistoryLogs");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersHistoryLogs_HistoryLogs_HistoryLogsId",
                table: "UsersHistoryLogs",
                column: "HistoryLogsId",
                principalTable: "HistoryLogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersHistoryLogs_Users_UserId",
                table: "UsersHistoryLogs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
