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