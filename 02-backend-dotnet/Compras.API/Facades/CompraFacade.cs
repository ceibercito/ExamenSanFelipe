using Microsoft.EntityFrameworkCore;
using Shared.Data;
using Shared.Dtos;
using Shared.Entities;

namespace Compras.API.Facades;

/// <summary>
/// PATRÓN FACADE: expone una operación simple (RegistrarCompraAsync) que por
/// debajo coordina varios pasos complejos en UNA transacción:
///   1. Inserta CompraCab + CompraDet
///   2. Actualiza Costo y PrecioVenta del producto (PrecioVenta = Costo * 1.35)
///   3. Inserta el Movimiento de Entrada (tipo 1)
/// El controlador no conoce esta complejidad: solo llama a la fachada.
/// </summary>
public class CompraFacade : ICompraFacade
{
    private readonly AppDbContext _db;
    private const decimal IGV = 0.18m;
    private const decimal MARGEN = 1.35m;

    public CompraFacade(AppDbContext db) => _db = db;

    public async Task<int> RegistrarCompraAsync(CompraCreateDto dto)
    {
        if (dto.Detalles is null || dto.Detalles.Count == 0)
            throw new ArgumentException("La compra debe tener al menos un producto.");

        // EnableRetryOnFailure exige que las transacciones manuales se ejecuten
        // dentro de una execution strategy (como unidad reintentable).
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                // 1) Cabecera (los totales se acumulan con el detalle)
                var cab = new CompraCab { FecRegistro = DateTime.Now };
                _db.CompraCab.Add(cab);
                await _db.SaveChangesAsync();

                decimal totalSub = 0, totalIgv = 0, totalGeneral = 0;

                // 2) Movimiento de Entrada asociado a esta compra
                var movCab = new MovimientoCab
                {
                    FecRegistro = DateTime.Now,
                    IdTipoMovimiento = 1,             // Entrada
                    IdDocumentoOrigen = cab.IdCompraCab
                };
                _db.MovimientoCab.Add(movCab);
                await _db.SaveChangesAsync();

                foreach (var item in dto.Detalles)
                {
                    var producto = await _db.Productos.FindAsync(item.IdProducto)
                        ?? throw new InvalidOperationException($"Producto {item.IdProducto} no existe.");

                    var sub = item.Cantidad * item.Precio;
                    var igv = sub * IGV;
                    var tot = sub + igv;

                    _db.CompraDet.Add(new CompraDet
                    {
                        IdCompraCab = cab.IdCompraCab,
                        IdProducto = item.IdProducto,
                        Cantidad = item.Cantidad,
                        Precio = item.Precio,
                        SubTotal = sub,
                        Igv = igv,
                        Total = tot
                    });

                    // Actualiza costo y precio de venta del producto
                    producto.Costo = item.Precio;
                    producto.PrecioVenta = Math.Round(item.Precio * MARGEN, 2);

                    // Detalle del movimiento de entrada
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

                // 3) Actualiza los totales de la cabecera
                cab.SubTotal = totalSub;
                cab.Igv = totalIgv;
                cab.Total = totalGeneral;

                await _db.SaveChangesAsync();
                await tx.CommitAsync();
                return cab.IdCompraCab;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        });
    }

    public async Task<List<CompraListDto>> ListarComprasAsync()
    {
        return await _db.CompraCab
            .OrderByDescending(c => c.FecRegistro)
            .Select(c => new CompraListDto(c.IdCompraCab, c.FecRegistro, c.SubTotal, c.Igv, c.Total))
            .ToListAsync();
    }
}
