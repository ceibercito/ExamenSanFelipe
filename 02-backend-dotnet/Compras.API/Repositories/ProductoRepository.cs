using Microsoft.EntityFrameworkCore;
using Shared.Data;
using Shared.Dtos;
using Shared.Entities;

namespace Compras.API.Repositories;

public class ProductoRepository : IProductoRepository
{
    private readonly AppDbContext _db;

    public ProductoRepository(AppDbContext db) => _db = db;

    public async Task<List<ProductoDto>> ListarConStockAsync()
    {
        // Stock = entradas (tipo 1) - salidas (tipo 2), calculado desde Movimiento.
        var query =
            from p in _db.Productos
            select new ProductoDto(
                p.IdProducto,
                p.NombreProducto,
                p.NroLote,
                p.Costo,
                p.PrecioVenta,
                (from md in _db.MovimientoDet
                 join mc in _db.MovimientoCab on md.IdMovimientoCab equals mc.IdMovimientoCab
                 where md.IdProducto == p.IdProducto
                 select (mc.IdTipoMovimiento == 1 ? md.Cantidad : -md.Cantidad))
                 .Sum());

        return await query.ToListAsync();
    }

    public async Task<Producto?> ObtenerAsync(int id)
        => await _db.Productos.FirstOrDefaultAsync(p => p.IdProducto == id);

    public async Task<Producto> CrearAsync(Producto producto)
    {
        _db.Productos.Add(producto);
        await _db.SaveChangesAsync();
        return producto;
    }

    public async Task<Producto> ActualizarAsync(Producto producto)
    {
        _db.Productos.Update(producto);
        await _db.SaveChangesAsync();
        return producto;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var producto = await _db.Productos.FindAsync(id);
        if (producto is null) return false;
        _db.Productos.Remove(producto);
        await _db.SaveChangesAsync();
        return true;
    }
}
