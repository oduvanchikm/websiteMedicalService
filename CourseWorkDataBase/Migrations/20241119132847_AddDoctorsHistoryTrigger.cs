using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseWorkDataBase.Migrations
{
    /// <inheritdoc />
    public partial class AddDoctorsHistoryTrigger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var createFunction = @"
            CREATE OR REPLACE FUNCTION DoctorTriggerFunction()
            RETURNS TRIGGER AS
                $$
            DECLARE
            UserId        BIGINT;
            HistoryLogId  BIGINT;
            TableName     TEXT;
            OperationType TEXT;
            BEGIN
            IF TG_OP = 'INSERT' THEN
            UserId := NEW.""UserId"";
            TableName := TG_TABLE_NAME;
            OperationType := 'INSERT';

            INSERT INTO ""HistoryLogs"" (""TableName"", ""OperationType"", ""ChangeTime"")
            VALUES (TableName,
                OperationType,
                NOW())
            RETURNING ""Id"" INTO HistoryLogId;

            INSERT INTO ""UsersHistoryLogs"" (""HistoryLogsId"", ""UserId"")
            VALUES (HistoryLogId, UserId);
            RETURN NEW;

            ELSIF TG_OP = 'UPDATE' THEN
            UserId := NEW.""UserId"";
            TableName := TG_TABLE_NAME;
            OperationType := 'UPDATE';

            INSERT INTO ""HistoryLogs"" (""TableName"", ""OperationType"", ""ChangeTime"")
            VALUES (TableName,
                OperationType,
                NOW())
            RETURNING ""Id"" INTO HistoryLogId;

            INSERT INTO ""UsersHistoryLogs"" (""HistoryLogsId"", ""UserId"")
            VALUES (HistoryLogId, UserId);
            RETURN NEW;

            ELSIF TG_OP = 'DELETE' THEN
            UserId := OLD.""UserId"";
            TableName := TG_TABLE_NAME;
            OperationType := 'DELETE';

            INSERT INTO ""HistoryLogs"" (""TableName"", ""OperationType"", ""ChangeTime"")
            VALUES (TableName,
                OperationType,
                NOW())
            RETURNING ""Id"" INTO HistoryLogId;

            INSERT INTO ""UsersHistoryLogs"" (""HistoryLogsId"", ""UserId"")
            VALUES (HistoryLogId, UserId);
            RETURN OLD;

            END IF;
            END;
                $$ LANGUAGE plpgsql;
            ";
            
            migrationBuilder.Sql(createFunction);

            var createTrigger = @"
           CREATE TRIGGER DoctorTrigger
    AFTER INSERT OR UPDATE OR DELETE
    ON ""Doctors""
    FOR EACH ROW
EXECUTE PROCEDURE DoctorTriggerFunction();
";
            migrationBuilder.Sql(createTrigger);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var dropTrigger = @"
           DROP TRIGGER IF EXISTS ""DoctorTrigger"" ON ""Doctors"";
           ";

            migrationBuilder.Sql(dropTrigger);
            var dropFunction = @"
           DROP FUNCTION IF EXISTS DoctorTriggerFunction();
           ";

            migrationBuilder.Sql(dropFunction);
        }
    }
}
