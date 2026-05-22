/* =====================================================================
   Archivo 02: datos iniciales (seed)
   ===================================================================== */
USE VentasComprasDB;
GO

/* Usuario de prueba para login.
   Username: admin
   Password: admin123   (hash BCrypt $2b$10$...)
   El backend valida con BCrypt.Verify. */
IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE Username = 'admin')
BEGIN
    INSERT INTO dbo.Usuarios (Username, PasswordHash, Nombre, Activo)
    VALUES ('admin',
            '$2b$10$JZfwluqpMCT7rWCcKZ80HehiNKg1QuP1H3tbVgRRIkxtudfhftlWW',
            'Administrador', 1);
END
GO

/* Algunos productos de ejemplo (sin stock aún; el stock se genera al comprar) */
IF NOT EXISTS (SELECT 1 FROM dbo.Productos)
BEGIN
    INSERT INTO dbo.Productos (Nombre_producto, NroLote, Costo, PrecioVenta) VALUES
        ('Laptop HP 240 G9',    'LOTE-001', 0, 0),
        ('Mouse Logitech M170', 'LOTE-002', 0, 0),
        ('Teclado Redragon K552','LOTE-003', 0, 0);
END
GO

PRINT 'Datos iniciales insertados.';
GO
