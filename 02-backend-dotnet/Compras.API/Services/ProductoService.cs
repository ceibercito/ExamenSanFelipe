using Compras.API.Repositories;
using Shared.Dtos;
using Shared.Entities;

namespace Compras.API.Services;

/// <summary>
/// Lógica de negocio de productos (SRP). Depende de la abstracción del
/// repositorio (DIP), no de la implementación concreta.
/// </summary>
public class ProductoService : IProductoService
{
    private readonly IProductoRepository _repo;

    public ProductoService(IProductoRepository repo) => _repo = repo;

    public Task<List<ProductoDto>> ListarAsync() => _repo.ListarConStockAsync();

    public async Task<ProductoDto?> ObtenerAsync(int id)
    {
        var p = await _repo.ObtenerAsync(id);
        return p is null ? null
            : new ProductoDto(p.IdProducto, p.NombreProducto, p.NroLote, p.Costo, p.PrecioVenta, 0);
    }

    public async Task<ProductoDto> RegistrarAsync(ProductoCreateDto dto)
    {
        var producto = new Producto
        {
            NombreProducto = dto.NombreProducto,
            NroLote = dto.NroLote,
            Costo = dto.Costo,
            PrecioVenta = dto.PrecioVenta,
            FecRegistro = DateTime.Now
        };
        var creado = await _repo.CrearAsync(producto);
        return new ProductoDto(creado.IdProducto, creado.NombreProducto, creado.NroLote,
                               creado.Costo, creado.PrecioVenta, 0);
    }

    public async Task<ProductoDto?> ActualizarAsync(ProductoUpdateDto dto)
    {
        var producto = await _repo.ObtenerAsync(dto.IdProducto);
        if (producto is null) return null;

        producto.NombreProducto = dto.NombreProducto;
        producto.NroLote = dto.NroLote;
        producto.Costo = dto.Costo;
        producto.PrecioVenta = dto.PrecioVenta;

        var actualizado = await _repo.ActualizarAsync(producto);
        return new ProductoDto(actualizado.IdProducto, actualizado.NombreProducto, actualizado.NroLote,
                               actualizado.Costo, actualizado.PrecioVenta, 0);
    }

    public Task<bool> EliminarAsync(int id) => _repo.EliminarAsync(id);
}
