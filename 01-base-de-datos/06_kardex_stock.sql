/* =====================================================================
   Archivo 06: Kardex y stock
   ===================================================================== */
USE VentasComprasDB;
GO

/* ---------------------------------------------------------------------
   KARDEX: resumen por producto con stock actual, costo y precio de venta.
   Es lo que muestra la vista principal del Kardex en el frontend.
   --------------------------------------------------------------------- */
SELECT
    p.Id_producto,
    p.Nombre_producto,
    p.Costo,
    p.PrecioVenta,
    ISNULL(SUM(CASE WHEN mc.Id_TipoMovimiento = 1 THEN md.Cantidad
                    WHEN mc.Id_TipoMovimiento = 2 THEN -md.Cantidad
                    ELSE 0 END), 0) AS StockActual
FROM dbo.Productos p
LEFT JOIN dbo.MovimientoDet md ON md.Id_Producto = p.Id_producto
LEFT JOIN dbo.MovimientoCab mc ON mc.Id_MovimientoCab = md.Id_movimientocab
GROUP BY p.Id_producto, p.Nombre_producto, p.Costo, p.PrecioVenta
ORDER BY p.Nombre_producto;
GO

/* ---------------------------------------------------------------------
   DETALLE DE MOVIMIENTOS de un producto (lo que se ve en el modal):
   Fecha registro, tipo movimiento (texto) y cantidad.
   --------------------------------------------------------------------- */
-- Reemplazar @Id_producto por el id deseado
DECLARE @Id_producto INT = 1;

SELECT
    mc.Fec_registro                                   AS FechaRegistro,
    CASE mc.Id_TipoMovimiento
         WHEN 1 THEN 'Entrada'
         WHEN 2 THEN 'Salida'
         ELSE 'Otro' END                              AS TipoMovimiento,
    md.Cantidad,
    mc.Id_DocumentoOrigen                             AS DocumentoOrigen
FROM dbo.MovimientoDet md
INNER JOIN dbo.MovimientoCab mc ON mc.Id_MovimientoCab = md.Id_movimientocab
WHERE md.Id_Producto = @Id_producto
ORDER BY mc.Fec_registro DESC;
GO
