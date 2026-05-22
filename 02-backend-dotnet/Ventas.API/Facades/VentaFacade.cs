using Microsoft.EntityFrameworkCore;
using Shared.Data;
using Shared.Dtos;
using Shared.Entities;

namespace Ventas.API.Facades;

/// <summary>
/// PATRÓN FACADE para registrar una venta. Coordina en una transacción:
///   1. Valida stock disponible por producto (no permite vender más que el stock).
///   2. Calcula SubTotal = Cantidad * PrecioVenta, Igv = SubTotal * 0.18, Total.
///   3. Inserta VentaCab + VentaDet.
///   4. Inserta Movimiento de Salida (tipo 2).
/// </summary>
public class VentaFacade : IVentaFacade
{
    private readonly AppDbContext _db;
    private const decimal IGV = 0.18m;

    public VentaFacade(AppDbContext db) => _db = db;

    private async Task<int> ObtenerStockAsync(int idProducto)
    {
        return await (from md in _db.MovimientoDet
                      join mc in _db.MovimientoCab on md.IdMovimientoCab equals mc.IdMovimientoCab
                      where md.IdProducto == idProducto
                      select (mc.IdTipoMovimiento == 1 ? md.Cantidad : -md.Cantidad))
                     .SumAsync();
    }

    public async Task<int> RegistrarVentaAsync(VentaCreateDto dto)
    {
        if (dto.Detalles is null || dto.Detalles.Count == 0)
            throw new ArgumentException("La venta debe tener al menos un producto.");

        // EnableRetryOnFailure exige que las transacciones manuales se ejecuten
        // dentro de una execution strategy (como unidad reintentable).
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                // 1) Validación de stock para todos los productos ANTES de guardar
                foreach (var item in dto.Detalles)
                {
                    var stock = await ObtenerStockAsync(item.IdProducto);
                    if (item.Cantidad > stock)
                    {
                        var producto = await _db.Productos.FindAsync(item.IdProducto);
                        var nombre = producto?.NombreProducto ?? $"#{item.IdProducto}";
                        throw new InvalidOperationException(
                            $"La cantidad ({item.Cantidad}) no debe ser mayor al stock disponible ({stock}) del producto {nombre}.");
                    }
                }

                var cab = new VentaCab { FecRegistro = DateTime.Now };
                _db.VentaCab.Add(cab);
                await _db.SaveChangesAsync();

                var movCab = new MovimientoCab
                {
                    FecRegistro = DateTime.Now,
                    IdTipoMovimiento = 2,             // Salida
                    IdDocumentoOrigen = cab.IdVentaCab
                };
                _db.MovimientoCab.Add(movCab);
                await _db.SaveChangesAsync();

                decimal totalSub = 0, totalIgv = 0, totalGeneral = 0;

                foreach (var item in dto.Detalles)
                {
                    var producto = await _db.Productos.FindAsync(item.IdProducto)
                        ?? throw new InvalidOperationException($"Producto {item.IdProducto} no existe.");

                    var precio = producto.PrecioVenta;
                    var sub = item.Cantidad * precio;
                    var igv = sub * IGV;
                    var tot = sub + igv;

                    _db.VentaDet.Add(new VentaDet
                    {
                        IdVentaCab = cab.IdVentaCab,
                        IdProducto = item.IdProducto,
                        Cantidad = item.Cantidad,
                        Precio = precio,
                        SubTotal = sub,
                        Igv = igv,
                        Total = tot
                    });

                    _db.MovimientoDet.Add(new MovimientoDet
                    {
                        IdMovimientoCab = movCab.IdMovimientoCab,
                        IdProducto = item.IdProducto,
                        Cantidad = item.Cantidad
                    });

                    totalSub += sub;
                    totalIgv += igv;
                    totalGeneral += tot;
                }

                cab.SubTotal = totalSub;
                cab.Igv = totalIgv;
                cab.Total = totalGeneral;

                await _db.SaveChangesAsync();
                await tx.CommitAsync();
                return cab.IdVentaCab;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        });
    }

    public async Task<List<VentaListDto>> ListarVentasAsync()
    {
        return await _db.VentaCab
            .OrderByDescending(v => v.FecRegistro)
            .Select(v => new VentaListDto(v.IdVentaCab, v.FecRegistro, v.SubTotal, v.Igv, v.Total))
            .ToListAsync();
    }
}
