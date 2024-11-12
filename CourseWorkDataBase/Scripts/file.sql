CREATE OR REPLACE FUNCTION GetDoctorsBySpecialty(p_SpecialtyId BIGINT)
    RETURNS TABLE(ID BIGINT, FirstName TEXT, FamilyName TEXT, SpecialtyId BIGINT, NameSpecialty TEXT) AS
$$
BEGIN
    RETURN QUERY
        SELECT
            d."ID",
            d."FirstName",
            d."FamilyName",
            d."SpecialtyID",
            s."NameSpecialty"
        FROM
            "Doctors" d
                INNER JOIN
            "Specialties" s
            ON d."SpecialtyID" = s."Id"
        WHERE
            (p_SpecialtyId = 0 OR d."SpecialtyID" = p_SpecialtyId);
END;
$$ LANGUAGE plpgsql;

-- SELECT * FROM GetDoctorsBySpecialty(0);
-- SELECT * FROM GetDoctorsBySpecialty(1);