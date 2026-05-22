/* =====================================================================
   Archivo 04: registro y listado de Compras
   Demuestra la transacción completa que luego replica el backend .NET.
   ===================================================================== */
USE VentasComprasDB;
GO

/* ---------------------------------------------------------------------
   Stored procedure: registrar una compra completa.
   - Inserta CompraCab + CompraDet
   - Actualiza Costo y PrecioVenta del producto (PrecioVenta = Costo * 1.35)
   - Inserta Movimiento de tipo Entrada (1)
   Para simplificar el ejemplo recibe un solo producto; el backend maneja
   la lista de detalles dentro de una misma transacción.
   --------------------------------------------------------------------- */
IF OBJECT_ID('dbo.sp_RegistrarCompraSimple', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_RegistrarCompraSimple;
GO
CREATE PROCEDURE dbo.sp_RegistrarCompraSimple
    @Id_producto INT,
    @Cantidad    INT,
    @Precio      DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @SubTotal DECIMAL(18,2) = @Cantidad * @Precio;
        DECLARE @Igv      DECIMAL(18,2) = @SubTotal * 0.18;
        DECLARE @Total    DECIMAL(18,2) = @SubTotal + @Igv;

        -- 1) Cabecera de compra
        INSERT INTO dbo.CompraCab (FecRegistro, SubTotal, Igv, Total)
        VALUES (GETDATE(), @SubTotal, @Igv, @Total);
        DECLARE @Id_CompraCab INT = SCOPE_IDENTITY();

        -- 2) Detalle de compra
        INSERT INTO dbo.CompraDet (Id_CompraCab, Id_producto, Cantidad, Precio, Sub_Total, Igv, Total)
        VALUES (@Id_CompraCab, @Id_producto, @Cantidad, @Precio, @SubTotal, @Igv, @Total);

        -- 3) Actualizar costo y precio de venta del producto (PrecioVenta = Costo * 1.35)
        UPDATE dbo.Productos
        SET Costo = @Precio,
            PrecioVenta = CAST(@Precio * 1.35 AS DECIMAL(18,2))
        WHERE Id_producto = @Id_producto;

        -- 4) Movimiento de Entrada
        INSERT INTO dbo.MovimientoCab (Fec_registro, Id_TipoMovimiento, Id_DocumentoOrigen)
        VALUES (GETDATE(), 1, @Id_CompraCab);
        DECLARE @Id_MovCab INT = SCOPE_IDENTITY();

        INSERT INTO dbo.MovimientoDet (Id_movimientocab, Id_Producto, Cantidad)
        VALUES (@Id_MovCab, @Id_producto, @Cantidad);

        COMMIT TRANSACTION;
        SELECT @Id_CompraCab AS Id_CompraCab;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

/* -------------------- LISTAR compras (cabecera + detalle) -------------------- */
-- Cabeceras
SELECT cc.Id_CompraCab, cc.FecRegistro, cc.SubTotal, cc.Igv, cc.Total
FROM dbo.CompraCab cc
ORDER BY cc.FecRegistro DESC;
GO

-- Detalle de una compra
-- SELECT cd.*, p.Nombre_producto
-- FROM dbo.CompraDet cd
-- INNER JOIN dbo.Productos p ON p.Id_producto = cd.Id_producto
-- WHERE cd.Id_CompraCab = @Id_CompraCab;
GO

/* Ejemplo de uso:
   EXEC dbo.sp_RegistrarCompraSimple @Id_producto = 1, @Cantidad = 10, @Precio = 1800.00;
*/
