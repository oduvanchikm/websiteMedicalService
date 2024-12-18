using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseWorkDataBase.Migrations
{
    /// <inheritdoc />
    public partial class trigger12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var createFunction = @"
CREATE OR REPLACE FUNCTION PatientTriggerFunction()
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
    END IF;
END;
$$ LANGUAGE plpgsql;
";
            migrationBuilder.Sql(createFunction);
            
            var createTrigger = @"

CREATE TRIGGER PatientTrigger
    AFTER INSERT OR UPDATE
    ON ""Patients""
    FOR EACH ROW
EXECUTE PROCEDURE PatientTriggerFunction();
";
            migrationBuilder.Sql(createTrigger);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var dropTrigger = @"
           
DROP TRIGGER IF EXISTS PatientTrigger ON ""Patients"";
           ";

            migrationBuilder.Sql(dropTrigger);
            
            var dropFunction = @"
           DROP FUNCTION IF EXISTS PatientTriggerFunction();
           ";

            migrationBuilder.Sql(dropFunction);
        }
    }
}
