﻿CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

CREATE TABLE "Clinics" (
    "Id" bigint GENERATED BY DEFAULT AS IDENTITY,
    "Address" text NOT NULL,
    "PhoneNumber" text NOT NULL,
    CONSTRAINT "PK_Clinics" PRIMARY KEY ("Id")
);

CREATE TABLE "HistoryLogs" (
    "Id" bigint GENERATED BY DEFAULT AS IDENTITY,
    "TableName" text NOT NULL,
    "OperationType" text NOT NULL,
    "ChangeTime" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_HistoryLogs" PRIMARY KEY ("Id")
);

CREATE TABLE "Medications" (
    "MedicationId" bigint GENERATED BY DEFAULT AS IDENTITY,
    "Name" character varying(100) NOT NULL,
    "Description" character varying(500) NOT NULL,
    CONSTRAINT "PK_Medications" PRIMARY KEY ("MedicationId")
);

CREATE TABLE "Roles" (
    "Id" bigint GENERATED BY DEFAULT AS IDENTITY,
    "Name" text NOT NULL,
    CONSTRAINT "PK_Roles" PRIMARY KEY ("Id")
);

CREATE TABLE "Specialties" (
    "Id" bigint GENERATED BY DEFAULT AS IDENTITY,
    "NameSpecialty" text NOT NULL,
    "Description" text,
    CONSTRAINT "PK_Specialties" PRIMARY KEY ("Id")
);

CREATE TABLE "Statuses" (
    "Id" bigint GENERATED BY DEFAULT AS IDENTITY,
    "Name" text NOT NULL,
    CONSTRAINT "PK_Statuses" PRIMARY KEY ("Id")
);

CREATE TABLE "Users" (
    "Id" bigint GENERATED BY DEFAULT AS IDENTITY,
    "Email" character varying(50) NOT NULL,
    "Password" text NOT NULL,
    "RoleId" bigint NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Users" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Users_Roles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "Roles" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "Doctors" (
    "ID" bigint GENERATED BY DEFAULT AS IDENTITY,
    "FirstName" text NOT NULL,
    "FamilyName" text NOT NULL,
    "SpecialtyID" bigint NOT NULL,
    "UserId" bigint NOT NULL,
    "ClinicId" bigint,
    CONSTRAINT "PK_Doctors" PRIMARY KEY ("ID"),
    CONSTRAINT "FK_Doctors_Clinics_ClinicId" FOREIGN KEY ("ClinicId") REFERENCES "Clinics" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Doctors_Specialties_SpecialtyID" FOREIGN KEY ("SpecialtyID") REFERENCES "Specialties" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Doctors_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Patients" (
    "Id" bigint GENERATED BY DEFAULT AS IDENTITY,
    "FirstName" text NOT NULL,
    "FamilyName" text NOT NULL,
    "Gender" text NOT NULL,
    "Date" timestamp with time zone NOT NULL,
    "UserId" bigint NOT NULL,
    CONSTRAINT "PK_Patients" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Patients_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "UsersHistoryLogs" (
    "HistoryLogsId" bigint NOT NULL,
    "UserId" bigint NOT NULL,
    CONSTRAINT "PK_UsersHistoryLogs" PRIMARY KEY ("UserId", "HistoryLogsId"),
    CONSTRAINT "FK_UsersHistoryLogs_HistoryLogs_HistoryLogsId" FOREIGN KEY ("HistoryLogsId") REFERENCES "HistoryLogs" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_UsersHistoryLogs_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AppointmentSlots" (
    "Id" bigint GENERATED BY DEFAULT AS IDENTITY,
    "StartTime" timestamp with time zone NOT NULL,
    "EndTime" timestamp with time zone NOT NULL,
    "DoctorId" bigint NOT NULL,
    "IsBooked" boolean NOT NULL,
    CONSTRAINT "PK_AppointmentSlots" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_AppointmentSlots_Doctors_DoctorId" FOREIGN KEY ("DoctorId") REFERENCES "Doctors" ("ID") ON DELETE CASCADE
);

CREATE TABLE "Appointments" (
    "Id" bigint GENERATED BY DEFAULT AS IDENTITY,
    "PatientId" bigint NOT NULL,
    "AppointmentSlotId" bigint NOT NULL,
    "Date" timestamp with time zone NOT NULL,
    "StatusId" bigint NOT NULL,
    CONSTRAINT "PK_Appointments" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Appointments_AppointmentSlots_AppointmentSlotId" FOREIGN KEY ("AppointmentSlotId") REFERENCES "AppointmentSlots" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Appointments_Patients_PatientId" FOREIGN KEY ("PatientId") REFERENCES "Patients" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Appointments_Statuses_StatusId" FOREIGN KEY ("StatusId") REFERENCES "Statuses" ("Id") ON DELETE CASCADE
);

CREATE TABLE "MedicalRecords" (
    "Id" bigint GENERATED BY DEFAULT AS IDENTITY,
    "Description" text NOT NULL,
    "Diagnosis" text NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdateAt" timestamp with time zone NOT NULL,
    "AppointmentId" bigint NOT NULL,
    CONSTRAINT "PK_MedicalRecords" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_MedicalRecords_Appointments_AppointmentId" FOREIGN KEY ("AppointmentId") REFERENCES "Appointments" ("Id") ON DELETE CASCADE
);

CREATE TABLE "MedicalRecordMedications" (
    "MedicalRecordId" bigint NOT NULL,
    "MedicationId" bigint NOT NULL,
    CONSTRAINT "PK_MedicalRecordMedications" PRIMARY KEY ("MedicalRecordId", "MedicationId"),
    CONSTRAINT "FK_MedicalRecordMedications_MedicalRecords_MedicalRecordId" FOREIGN KEY ("MedicalRecordId") REFERENCES "MedicalRecords" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_MedicalRecordMedications_Medications_MedicationId" FOREIGN KEY ("MedicationId") REFERENCES "Medications" ("MedicationId") ON DELETE CASCADE
);

INSERT INTO "Roles" ("Id", "Name")
VALUES (1, 'Admin');
INSERT INTO "Roles" ("Id", "Name")
VALUES (2, 'Doctor');
INSERT INTO "Roles" ("Id", "Name")
VALUES (3, 'Patient');

INSERT INTO "Statuses" ("Id", "Name")
VALUES (1, 'Booked');
INSERT INTO "Statuses" ("Id", "Name")
VALUES (2, 'Not booked');
INSERT INTO "Statuses" ("Id", "Name")
VALUES (3, 'Canceled');

INSERT INTO "Users" ("Id", "CreatedAt", "Email", "Password", "RoleId")
VALUES (1, TIMESTAMPTZ '2024-10-01T00:00:00+00:00', 'admin@gmail.com', '$2a$11$o.sTnyjh8Mr9ArOWpr5Q..rsRPFHJ7EJ6pIeFUyVEfP2fe5b1riHm', 1);

CREATE UNIQUE INDEX "IX_Appointments_AppointmentSlotId" ON "Appointments" ("AppointmentSlotId");

CREATE INDEX "IX_Appointments_PatientId" ON "Appointments" ("PatientId");

CREATE INDEX "IX_Appointments_StatusId" ON "Appointments" ("StatusId");

CREATE INDEX "IX_AppointmentSlots_DoctorId" ON "AppointmentSlots" ("DoctorId");

CREATE INDEX "IX_Doctors_ClinicId" ON "Doctors" ("ClinicId");

CREATE INDEX "IX_Doctors_SpecialtyID" ON "Doctors" ("SpecialtyID");

CREATE UNIQUE INDEX "IX_Doctors_UserId" ON "Doctors" ("UserId");

CREATE INDEX "IX_MedicalRecordMedications_MedicationId" ON "MedicalRecordMedications" ("MedicationId");

CREATE INDEX "IX_MedicalRecords_AppointmentId" ON "MedicalRecords" ("AppointmentId");

CREATE UNIQUE INDEX "IX_Patients_UserId" ON "Patients" ("UserId");

CREATE INDEX "IX_Users_RoleId" ON "Users" ("RoleId");

CREATE INDEX "IX_UsersHistoryLogs_HistoryLogsId" ON "UsersHistoryLogs" ("HistoryLogsId");

SELECT setval(
    pg_get_serial_sequence('"Roles"', 'Id'),
    GREATEST(
        (SELECT MAX("Id") FROM "Roles") + 1,
        nextval(pg_get_serial_sequence('"Roles"', 'Id'))),
    false);
SELECT setval(
    pg_get_serial_sequence('"Statuses"', 'Id'),
    GREATEST(
        (SELECT MAX("Id") FROM "Statuses") + 1,
        nextval(pg_get_serial_sequence('"Statuses"', 'Id'))),
    false);
SELECT setval(
    pg_get_serial_sequence('"Users"', 'Id'),
    GREATEST(
        (SELECT MAX("Id") FROM "Users") + 1,
        nextval(pg_get_serial_sequence('"Users"', 'Id'))),
    false);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20241214173248_new', '9.0.0-rc.2.24474.1');


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

    ELSIF TG_OP = 'DELETE' THEN

        UserId := OLD."Id";
        TableName := TG_TABLE_NAME;
        OperationType := 'DELETE';

        INSERT INTO "HistoryLogs" ("TableName", "OperationType", "ChangeTime")
        VALUES (TableName,
                OperationType,
                NOW())
        RETURNING "Id" INTO HistoryLogId;

        INSERT INTO "UsersHistoryLogs" ("HistoryLogsId", "UserId")
        VALUES (HistoryLogId, UserId);

        RETURN OLD;
    END IF;
    RETURN NULL;
END;
$$ LANGUAGE plpgsql;




CREATE TRIGGER UserTrigger
    AFTER INSERT OR UPDATE OR DELETE
    ON "Users"
    FOR EACH ROW
EXECUTE PROCEDURE UserTriggerFunction();


INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20241214173832_firstTrigger', '9.0.0-rc.2.24474.1');


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

    ELSIF TG_OP = 'DELETE' THEN
        UserId := OLD."UserId";
        TableName := TG_TABLE_NAME;
        OperationType := 'DELETE';

        INSERT INTO "HistoryLogs" ("TableName", "OperationType", "ChangeTime")
        VALUES (TableName,
                OperationType,
                NOW())
        RETURNING "Id" INTO HistoryLogId;

        INSERT INTO "UsersHistoryLogs" ("HistoryLogsId", "UserId")
        VALUES (HistoryLogId, UserId);
        RETURN OLD;

    END IF;
END;
$$ LANGUAGE plpgsql;




CREATE TRIGGER PatientTrigger
    AFTER INSERT OR UPDATE OR DELETE
    ON "Patients"
    FOR EACH ROW
EXECUTE PROCEDURE PatientTriggerFunction();


INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20241214174538_secondTrigger', '9.0.0-rc.2.24474.1');


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

    ELSIF TG_OP = 'DELETE' THEN
        UserId := OLD."UserId";
        TableName := TG_TABLE_NAME;
        OperationType := 'DELETE';

        INSERT INTO "HistoryLogs" ("TableName", "OperationType", "ChangeTime")
        VALUES (TableName,
                OperationType,
                NOW())
        RETURNING "Id" INTO HistoryLogId;

        INSERT INTO "UsersHistoryLogs" ("HistoryLogsId", "UserId")
        VALUES (HistoryLogId, UserId);
        RETURN OLD;

    END IF;
END;
$$ LANGUAGE plpgsql;




CREATE TRIGGER DoctorTrigger
    AFTER INSERT OR UPDATE OR DELETE
    ON "Doctors"
    FOR EACH ROW
EXECUTE PROCEDURE DoctorClinicTriggerFunction();


INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20241214174738_thirdTrigger', '9.0.0-rc.2.24474.1');

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20241216203535_final', '9.0.0-rc.2.24474.1');

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20241216204230_finalsql', '9.0.0-rc.2.24474.1');

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20241216204758_final1', '9.0.0-rc.2.24474.1');

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20241216204846_final2', '9.0.0-rc.2.24474.1');

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20241216205129_final3', '9.0.0-rc.2.24474.1');

COMMIT;

