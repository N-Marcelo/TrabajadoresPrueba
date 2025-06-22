CREATE OR ALTER PROCEDURE sp_ListarTrabajadores
AS
BEGIN
    SELECT 
        t.Id,
        t.TipoDocumento,
        t.NumeroDocumento,
        t.Nombres,
        t.Sexo,

        -- Claves for�neas necesarias para edici�n
        t.IdDepartamento,
        t.IdProvincia,
        t.IdDistrito,

        -- Nombres de entidades relacionadas para visualizaci�n
        d.NombreDepartamento,
        p.NombreProvincia,
        dis.NombreDistrito
    FROM Trabajadores t
    INNER JOIN Departamento d ON t.IdDepartamento = d.Id
    INNER JOIN Provincia p ON t.IdProvincia = p.Id
    INNER JOIN Distrito dis ON t.IdDistrito = dis.Id
END

exec sp_ListarTrabajadores