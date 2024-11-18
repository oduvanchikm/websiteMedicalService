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


DROP FUNCTION IF EXISTS get_patient_medical_records(BIGINT);

CREATE OR REPLACE FUNCTION get_patient_medical_records(p_patient_id BIGINT)
    RETURNS TABLE
            (
                patient_id                   BIGINT,
                first_name                   TEXT,
                family_name                  TEXT,
                appointment_id               BIGINT,
                medical_record_id            BIGINT,
                medical_record_description   TEXT,
                diagnosis                    TEXT,
                medication_id                BIGINT,
                medication_name              TEXT,
                medication_description       TEXT
            )
AS
$$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM "Patients" WHERE "Id" = p_patient_id) THEN
        RAISE EXCEPTION 'Patient with ID % not found.', p_patient_id;
    END IF;

    RETURN QUERY
        SELECT
            p."Id"                          AS patient_id,
            p."FirstName"::TEXT             AS first_name,
            p."FamilyName"::TEXT            AS family_name,
            a."Id"                          AS appointment_id,
            mr."Id"                         AS medical_record_id,
            mr."Description"::TEXT          AS medical_record_description,
            mr."Diagnosis"::TEXT            AS diagnosis,
            mrm."MedicationId"              AS medication_id,
            m."Name"::TEXT                  AS medication_name,
            m."Description"::TEXT           AS medication_description
        FROM
            "Patients" p
                INNER JOIN
            "Appointments" a ON a."PatientId" = p."Id"
                INNER JOIN
            "MedicalRecords" mr ON mr."Id" = a."Id"
                LEFT JOIN
            "MedicalRecordMedications" mrm ON mrm."MedicalRecordId" = mr."Id"
                LEFT JOIN
            "Medications" m ON m."MedicationId" = mrm."MedicationId"  
        WHERE
            p."Id" = p_patient_id
        ORDER BY
            a."Id", mr."Id", m."MedicationId"; 
END;
$$ LANGUAGE plpgsql;

SELECT *
FROM get_patient_medical_records(2::BIGINT);

--admin server
-- CREATE OR REPLACE FUNCTION AddDoctors(
--     p_email VARCHAR,
--     p_familyName VARCHAR,
--     p_firstName VARCHAR, 
--     p_personalNumber VARCHAR,
--     p_specialtyId BIGINT,
--     p_specialtyName VARCHAR,
--     p_specialtyDescription TEXT,
--     p_clinicId BIGINT,
--     p_clinicAddress VARCHAR,
--     p_clinicPhoneNumber VARCHAR
-- ) 
-- RETURNS  

SELECT version();
