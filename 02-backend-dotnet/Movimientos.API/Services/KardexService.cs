using Microsoft.EntityFrameworkCore;
using Shared.Data;
using Shared.Dtos;

namespace Movimientos.API.Services;

/// <summary>
/// Lógica del Kardex: stock por producto y detalle de movimientos.
/// </summary>
public class KardexService : IKardexService
{
    private readonly AppDbContext _db;

    public KardexService(AppDbContext db) => _db = db;

    public async Task<List<KardexDto>> ListarKardexAsync()
    {
        var query =
            from p in _db.Productos
            select new KardexDto(
                p.IdProducto,
                p.NombreProducto,
                (from md in _db.MovimientoDet
                 join mc in _db.MovimientoCab on md.IdMovimientoCab equals mc.IdMovimientoCab
                 where md.IdProducto == p.IdProducto
                 select (mc.IdTipoMovimiento == 1 ? md.Cantidad : -md.Cantidad)).Sum(),
                p.Costo,
                p.PrecioVenta);

        return await query.ToListAsync();
    }

    public async Task<List<MovimientoDto>> ListarMovimientosProductoAsync(int idProducto)
    {
        return await (from md in _db.MovimientoDet
                      join mc in _db.MovimientoCab on md.IdMovimientoCab equals mc.IdMovimientoCab
                      where md.IdProducto == idProducto
                      orderby mc.FecRegistro descending
                      select new MovimientoDto(
                          mc.FecRegistro,
                          mc.IdTipoMovimiento == 1 ? "Entrada" : "Salida",
                          md.Cantidad,
                          mc.IdDocumentoOrigen))
                     .ToListAsync();
    }
}
