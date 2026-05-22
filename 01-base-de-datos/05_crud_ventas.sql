/* =====================================================================
   Archivo 05: registro y listado de Ventas
   Valida stock, calcula totales e inserta movimiento de Salida.
   ===================================================================== */
USE VentasComprasDB;
GO

/* ---------------------------------------------------------------------
   Función: stock disponible de un producto (entradas - salidas)
   --------------------------------------------------------------------- */
IF OBJECT_ID('dbo.fn_StockProducto', 'FN') IS NOT NULL
    DROP FUNCTION dbo.fn_StockProducto;
GO
CREATE FUNCTION dbo.fn_StockProducto (@Id_producto INT)
RETURNS INT
AS
BEGIN
    DECLARE @stock INT;
    SELECT @stock = ISNULL(SUM(CASE WHEN mc.Id_TipoMovimiento = 1 THEN md.Cantidad
                                    WHEN mc.Id_TipoMovimiento = 2 THEN -md.Cantidad
                                    ELSE 0 END), 0)
    FROM dbo.MovimientoDet md
    INNER JOIN dbo.MovimientoCab mc ON mc.Id_MovimientoCab = md.Id_movimientocab
    WHERE md.Id_Producto = @Id_producto;
    RETURN @stock;
END
GO

/* ---------------------------------------------------------------------
   Stored procedure: registrar una venta (un producto, para el ejemplo).
   - Valida stock
   - Calcula SubTotal = Cantidad * PrecioVenta, Igv = SubTotal * 0.18, Total
   - Inserta VentaCab + VentaDet
   - Inserta Movimiento de tipo Salida (2)
   --------------------------------------------------------------------- */
IF OBJECT_ID('dbo.sp_RegistrarVentaSimple', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_RegistrarVentaSimple;
GO
CREATE PROCEDURE dbo.sp_RegistrarVentaSimple
    @Id_producto INT,
    @Cantidad    INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @stock INT = dbo.fn_StockProducto(@Id_producto);
        IF @Cantidad > @stock
        BEGIN
            ;THROW 50001, 'La cantidad no debe ser mayor al stock disponible.', 1;
        END

        DECLARE @PrecioVenta DECIMAL(18,2) =
            (SELECT PrecioVenta FROM dbo.Productos WHERE Id_producto = @Id_producto);

        DECLARE @SubTotal DECIMAL(18,2) = @Cantidad * @PrecioVenta;
        DECLARE @Igv      DECIMAL(18,2) = @SubTotal * 0.18;
        DECLARE @Total    DECIMAL(18,2) = @SubTotal + @Igv;

        INSERT INTO dbo.VentaCab (fecRegistro, SubTotal, Igv, Total)
        VALUES (GETDATE(), @SubTotal, @Igv, @Total);
        DECLARE @Id_VentaCab INT = SCOPE_IDENTITY();

        INSERT INTO dbo.VentaDet (Id_VentaCab, Id_producto, Cantidad, Precio, Sub_Total, Igv, Total)
        VALUES (@Id_VentaCab, @Id_producto, @Cantidad, @PrecioVenta, @SubTotal, @Igv, @Total);

        INSERT INTO dbo.MovimientoCab (Fec_registro, Id_TipoMovimiento, Id_DocumentoOrigen)
        VALUES (GETDATE(), 2, @Id_VentaCab);
        DECLARE @Id_MovCab INT = SCOPE_IDENTITY();

        INSERT INTO dbo.MovimientoDet (Id_movimientocab, Id_Producto, Cantidad)
        VALUES (@Id_MovCab, @Id_producto, @Cantidad);

        COMMIT TRANSACTION;
        SELECT @Id_VentaCab AS Id_VentaCab;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

/* -------------------- LISTAR ventas -------------------- */
SELECT vc.Id_VentaCab, vc.fecRegistro, vc.SubTotal, vc.Igv, vc.Total
FROM dbo.VentaCab vc
ORDER BY vc.fecRegistro DESC;
GO

/* Ejemplo:
   EXEC dbo.sp_RegistrarVentaSimple @Id_producto = 1, @Cantidad = 2;
*/
