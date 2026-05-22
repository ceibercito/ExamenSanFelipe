using Shared.Dtos;
using Shared.Entities;

namespace Compras.API.Repositories;

/// <summary>
/// Contrato de acceso a datos de productos (DIP + ISP de SOLID).
/// </summary>
public interface IProductoRepository
{
    Task<List<ProductoDto>> ListarConStockAsync();
    Task<Producto?> ObtenerAsync(int id);
    Task<Producto> CrearAsync(Producto producto);
    Task<Producto> ActualizarAsync(Producto producto);
    Task<bool> EliminarAsync(int id);
}
