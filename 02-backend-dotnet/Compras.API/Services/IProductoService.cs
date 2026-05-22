using Shared.Dtos;

namespace Compras.API.Services;

public interface IProductoService
{
    Task<List<ProductoDto>> ListarAsync();
    Task<ProductoDto?> ObtenerAsync(int id);
    Task<ProductoDto> RegistrarAsync(ProductoCreateDto dto);
    Task<ProductoDto?> ActualizarAsync(ProductoUpdateDto dto);
    Task<bool> EliminarAsync(int id);
}
