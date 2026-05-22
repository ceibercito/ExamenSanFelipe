/* =====================================================================
   Archivo 03: CRUD de Productos (insertar, listar, actualizar, eliminar)
   El examen pide explícitamente incluir estos scripts.
   ===================================================================== */
USE VentasComprasDB;
GO

/* -------------------- INSERTAR -------------------- */
-- INSERT INTO dbo.Productos (Nombre_producto, NroLote, Costo, PrecioVenta)
-- VALUES ('Monitor LG 24"', 'LOTE-010', 350.00, 472.50);

/* -------------------- LISTAR (con stock actual calculado del Kardex) -------------------- */
SELECT
    p.Id_producto,
    p.Nombre_producto,
    p.NroLote,
    p.Fec_registro,
    p.Costo,
    p.PrecioVenta,
    ISNULL(SUM(CASE WHEN mc.Id_TipoMovimiento = 1 THEN md.Cantidad
                    WHEN mc.Id_TipoMovimiento = 2 THEN -md.Cantidad
                    ELSE 0 END), 0) AS StockActual
FROM dbo.Productos p
LEFT JOIN dbo.MovimientoDet md ON md.Id_Producto = p.Id_producto
LEFT JOIN dbo.MovimientoCab mc ON mc.Id_MovimientoCab = md.Id_movimientocab
GROUP BY p.Id_producto, p.Nombre_producto, p.NroLote, p.Fec_registro, p.Costo, p.PrecioVenta
ORDER BY p.Id_producto;
GO

/* -------------------- LISTAR por Id -------------------- */
-- SELECT * FROM dbo.Productos WHERE Id_producto = @Id;

/* -------------------- ACTUALIZAR -------------------- */
-- UPDATE dbo.Productos
-- SET Nombre_producto = @Nombre, NroLote = @NroLote, Costo = @Costo, PrecioVenta = @PrecioVenta
-- WHERE Id_producto = @Id;

/* -------------------- ELIMINAR -------------------- */
-- Nota: si el producto tiene movimientos/compras/ventas, las FK lo impiden (integridad).
-- DELETE FROM dbo.Productos WHERE Id_producto = @Id;
GO
