-- -- GET SPECIALTY
CREATE OR REPLACE FUNCTION GetDoctorsBySpecialty(p_SpecialtyId BIGINT)
    RETURNS TABLE
            (
                ID            BIGINT,
                FirstName     TEXT,
                FamilyName    TEXT,
                SpecialtyId   BIGINT,
                NameSpecialty TEXT
            )
AS
$$
BEGIN
    RETURN QUERY
        SELECT d."ID", d."FirstName", d."FamilyName", d."SpecialtyID", s."NameSpecialty"
        FROM "Doctors" d
                 INNER JOIN
             "Specialties" s
             ON d."SpecialtyID" = s."Id"
        WHERE (p_SpecialtyId = 0 OR d."SpecialtyID" = p_SpecialtyId);
END;
$$ LANGUAGE plpgsql;

SELECT *
FROM GetDoctorsBySpecialty(0);
SELECT *
FROM GetDoctorsBySpecialty(1);
--

-- BOOK APPOINTMENT
CREATE OR REPLACE PROCEDURE BookAppointment(
    p_SlotId BIGINT,
    p_PatientId BIGINT
)
    LANGUAGE plpgsql
AS
$$
BEGIN
    IF NOT EXISTS(SELECT 1
                  FROM "AppointmentSlots"
                  WHERE "Id" = p_SlotId
                    AND NOT "IsBooked"
                    AND "StartTime" >= CURRENT_DATE) THEN
        RAISE EXCEPTION 'Slot is already booked or does not exist';
    END IF;

    UPDATE "AppointmentSlots"
    SET "IsBooked" = TRUE
    WHERE "Id" = p_SlotId;

    INSERT INTO "Appointments" ("PatientId", "AppointmentSlotId", "Date", "StatusId")
    VALUES (p_PatientId, p_SlotId, NOW(), 1);

EXCEPTION
    WHEN OTHERS THEN
        RAISE EXCEPTION 'An error occurred during booking: %', SQLERRM;
END;
$$;
--

-- GET MEDICAL RECORDS IN DOCTOR PAGE
DROP FUNCTION IF EXISTS get_patient_medical_records(BIGINT);
-- не работает естестевенно 
CREATE OR REPLACE FUNCTION get_patient_medical_records(p_patient_id BIGINT)
    RETURNS TABLE
            (
                patient_id                 BIGINT,
                first_name                 TEXT,
                family_name                TEXT,
                appointment_id             BIGINT,
                medical_record_id          BIGINT,
                medical_record_description TEXT,
                diagnosis                  TEXT,
                medication_id              BIGINT,
                medication_name            TEXT,
                medication_description     TEXT
            )
AS
$$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM "Patients" WHERE "Id" = p_patient_id) THEN
        RAISE EXCEPTION 'Patient with ID % not found.', p_patient_id;
    END IF;

    RETURN QUERY
        SELECT p."Id"                 AS patient_id,
               p."FirstName"::TEXT    AS first_name,
               p."FamilyName"::TEXT   AS family_name,
               a."Id"                 AS appointment_id,
               mr."Id"                AS medical_record_id,
               mr."Description"::TEXT AS medical_record_description,
               mr."Diagnosis"::TEXT   AS diagnosis,
               mrm."MedicationId"     AS medication_id,
               m."Name"::TEXT         AS medication_name,
               m."Description"::TEXT  AS medication_description
        FROM "Patients" p
                 INNER JOIN
             "Appointments" a ON a."PatientId" = p."Id"
                 INNER JOIN
             "MedicalRecords" mr ON mr."Id" = a."Id"
                 LEFT JOIN
             "MedicalRecordMedications" mrm ON mrm."MedicalRecordId" = mr."Id"
                 LEFT JOIN
             "Medications" m ON m."MedicationId" = mrm."MedicationId"
        WHERE p."Id" = p_patient_id
        ORDER BY a."Id", mr."Id", m."MedicationId";
END;
$$ LANGUAGE plpgsql;

SELECT *
FROM get_patient_medical_records(2::BIGINT);
--

SELECT version();

-- триггеры :)
-- TRIGGER USER TABLE
CREATE OR REPLACE FUNCTION UserTriggerFunction()
    RETURNS TRIGGER AS
$$
DECLARE
    UserId        BIGINT;
    HistoryLogId  BIGINT;
    TableName     TEXT;
    OperationType TEXT;

BEGIN
    IF TG_OP = 'INSERT' THEN
        UserId := NEW."Id";
        TableName := TG_TABLE_NAME;
        OperationType := 'INSERT';

        INSERT INTO "HistoryLogs" ("TableName", "OperationType", "ChangeTime")
        VALUES (TableName,
                OperationType,
                NOW())
        RETURNING "Id" INTO HistoryLogId;

        INSERT INTO "UsersHistoryLogs" ("HistoryLogsId", "UserId")
        VALUES (HistoryLogId, UserId);

        RETURN NEW;

    ELSIF TG_OP = 'UPDATE' THEN

        UserId := NEW."Id";
        TableName := TG_TABLE_NAME;
        OperationType := 'UPDATE';

        INSERT INTO "HistoryLogs" ("TableName", "OperationType", "ChangeTime")
        VALUES (TableName,
                OperationType,
                NOW())
        RETURNING "Id" INTO HistoryLogId;

        INSERT INTO "UsersHistoryLogs" ("HistoryLogsId", "UserId")
        VALUES (HistoryLogId, UserId);

        RETURN NEW;
    END IF;
    RETURN NULL;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER UserTrigger
    AFTER INSERT OR UPDATE
    ON "Users"
    FOR EACH ROW
EXECUTE PROCEDURE UserTriggerFunction();

SELECT *
FROM "HistoryLogs";
SELECT *
FROM "UsersHistoryLogs";
--

DROP TRIGGER IF EXISTS UserTrigger ON "Users";

-- TRIGGER PATIENT TALE
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
        UserId := NEW."UserId";
        TableName := TG_TABLE_NAME;
        OperationType := 'INSERT';

        INSERT INTO "HistoryLogs" ("TableName", "OperationType", "ChangeTime")
        VALUES (TableName,
                OperationType,
                NOW())
        RETURNING "Id" INTO HistoryLogId;

        INSERT INTO "UsersHistoryLogs" ("HistoryLogsId", "UserId")
        VALUES (HistoryLogId, UserId);
        RETURN NEW;

    ELSIF TG_OP = 'UPDATE' THEN
        UserId := NEW."UserId";
        TableName := TG_TABLE_NAME;
        OperationType := 'UPDATE';

        INSERT INTO "HistoryLogs" ("TableName", "OperationType", "ChangeTime")
        VALUES (TableName,
                OperationType,
                NOW())
        RETURNING "Id" INTO HistoryLogId;

        INSERT INTO "UsersHistoryLogs" ("HistoryLogsId", "UserId")
        VALUES (HistoryLogId, UserId);
        RETURN NEW;

    END IF;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER PatientTrigger
    AFTER INSERT OR UPDATE
    ON "Patients"
    FOR EACH ROW
EXECUTE PROCEDURE PatientTriggerFunction();

SELECT *
FROM "HistoryLogs";
SELECT *
FROM "UsersHistoryLogs";
--


DROP TRIGGER IF EXISTS PatientTrigger ON "Patients";

-- TRIGGER DOCTORS TABLE
CREATE OR REPLACE FUNCTION DoctorClinicTriggerFunction()
    RETURNS TRIGGER AS
$$
DECLARE
    UserId        BIGINT;
    HistoryLogId  BIGINT;
    DoctorId      BIGINT DEFAULT NULL;
    ClinicId      BIGINT DEFAULT NULL;
    TableName     TEXT;
    OperationType TEXT;
BEGIN
    IF TG_OP = 'INSERT' THEN
        UserId := NEW."UserId";
        DoctorId := NEW."ID";
        TableName := TG_TABLE_NAME;
        OperationType := 'INSERT';

        INSERT INTO "HistoryLogs" ("TableName", "OperationType", "ChangeTime")
        VALUES (TableName,
                OperationType,
                NOW())
        RETURNING "Id" INTO HistoryLogId;

        INSERT INTO "UsersHistoryLogs" ("HistoryLogsId", "UserId")
        VALUES (HistoryLogId, UserId);
        RETURN NEW;

    ELSIF TG_OP = 'UPDATE' THEN
        UserId := NEW."UserId";
        TableName := TG_TABLE_NAME;
        OperationType := 'UPDATE';

        INSERT INTO "HistoryLogs" ("TableName", "OperationType", "ChangeTime")
        VALUES (TableName,
                OperationType,
                NOW())
        RETURNING "Id" INTO HistoryLogId;

        INSERT INTO "UsersHistoryLogs" ("HistoryLogsId", "UserId")
        VALUES (HistoryLogId, UserId);
        RETURN NEW;
    END IF;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER DoctorTrigger
    AFTER INSERT OR UPDATE
    ON "Doctors"
    FOR EACH ROW
EXECUTE PROCEDURE DoctorClinicTriggerFunction();

SELECT *
FROM "HistoryLogs";
SELECT *
FROM "UsersHistoryLogs";

DROP TRIGGER IF EXISTS DoctorTrigger ON "Doctors";

select *
from "Users";

SELECT "UserId" AS userId
FROM "Doctors"
WHERE "ID" = (SELECT "ID"
              FROM "Doctors"
              WHERE "ClinicId" = 3);

select *
from "Clinics";

select *
from "Doctors";

select *
from "Users";

select *
from "UsersHistoryLogs";

select *
from "HistoryLogs";

select *
from "Roles";

-- INSERT INTO "Users" ("Id", "Email", "Password", "RoleId", "CreatedAt")
-- VALUES (1, 'admin@example.com', '$2a$11$o.sTnyjh8Mr9ArOWpr5Q..rsRPFHJ7EJ6pIeFUyVEfP2fe5b1riHm', 1,
--         '2024-12-14 17:50:33.767814 +00:00');
