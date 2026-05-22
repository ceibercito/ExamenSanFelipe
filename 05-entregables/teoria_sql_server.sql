/* =====================================================================
   EVALUACIÓN TEÓRICA - 1.3 SQL SERVER
   Respuestas a las 3 consultas pedidas.
   ===================================================================== */

/* ---------------------------------------------------------------------
   PREGUNTA 1: Top 10 productos más vendidos en el ÚLTIMO TRIMESTRE,
   con nombre, cantidad vendida y total de ingresos.
   Tablas: Productos(IdProducto, NombreProducto, Precio)
           Ventas(IdVenta, IdProducto, Cantidad, FechaVenta)
   --------------------------------------------------------------------- */
SELECT TOP 10
    p.NombreProducto,
    SUM(v.Cantidad)             AS CantidadVendida,
    SUM(v.Cantidad * p.Precio)  AS TotalIngresos
FROM Ventas v
INNER JOIN Productos p ON p.IdProducto = v.IdProducto
-- Último trimestre calendario completo (ej. si hoy es mayo, toma ene-mar):
WHERE v.FechaVenta >= DATEADD(QUARTER, DATEDIFF(QUARTER, 0, GETDATE()) - 1, 0)
  AND v.FechaVenta <  DATEADD(QUARTER, DATEDIFF(QUARTER, 0, GETDATE()), 0)
GROUP BY p.NombreProducto
ORDER BY CantidadVendida DESC;
-- Alternativa "últimos 3 meses": WHERE v.FechaVenta >= DATEADD(MONTH, -3, GETDATE())
GO


/* ---------------------------------------------------------------------
   PREGUNTA 2: Procedimiento que recibe 2 fechas y devuelve los préstamos
   de libros agrupados por libro y autor, INCLUYENDO los libros que no se
   prestaron, ordenado por el libro más prestado y apellidos del autor.
   Esquema asumido:
     Autores(IdAutor, Nombres, Apellidos)
     Libros(IdLibro, Titulo, IdAutor)
     Prestamos(IdPrestamo, IdLibro, FechaPrestamo)
   --------------------------------------------------------------------- */
CREATE OR ALTER PROCEDURE sp_PrestamosPorLibroAutor
    @FechaInicio DATE,
    @FechaFin    DATE
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        l.IdLibro,
        l.Titulo,
        a.Apellidos,
        a.Nombres,
        COUNT(pr.IdPrestamo) AS TotalPrestamos
    FROM Libros l
    INNER JOIN Autores a ON a.IdAutor = l.IdAutor
    -- LEFT JOIN + filtro de fechas en el ON: así se conservan los libros
    -- que NO tienen préstamos (saldrían con TotalPrestamos = 0).
    LEFT JOIN Prestamos pr
           ON pr.IdLibro = l.IdLibro
          AND pr.FechaPrestamo BETWEEN @FechaInicio AND @FechaFin
    GROUP BY l.IdLibro, l.Titulo, a.Apellidos, a.Nombres
    ORDER BY TotalPrestamos DESC, a.Apellidos ASC;
END
GO


/* ---------------------------------------------------------------------
   PREGUNTA 3: Trigger que, al ACTUALIZAR libros, registra en otra tabla
   los datos ANTES de actualizar, con usuario y fecha.
   --------------------------------------------------------------------- */
-- Tabla de auditoría
IF OBJECT_ID('dbo.Libros_Auditoria', 'U') IS NULL
CREATE TABLE dbo.Libros_Auditoria (
    IdAuditoria          INT IDENTITY(1,1) PRIMARY KEY,
    IdLibro              INT,
    Titulo               NVARCHAR(200),
    IdAutor              INT,
    UsuarioActualizacion NVARCHAR(128),
    FechaActualizacion   DATETIME
);
GO

CREATE OR ALTER TRIGGER trg_Libros_Update
ON dbo.Libros
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    -- La tabla virtual 'deleted' contiene los valores ANTES de la actualización.
    INSERT INTO dbo.Libros_Auditoria (IdLibro, Titulo, IdAutor, UsuarioActualizacion, FechaActualizacion)
    SELECT d.IdLibro, d.Titulo, d.IdAutor, SUSER_SNAME(), GETDATE()
    FROM deleted d;
END
GO
