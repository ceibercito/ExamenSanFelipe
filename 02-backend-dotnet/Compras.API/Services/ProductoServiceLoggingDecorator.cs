using Shared.Dtos;

namespace Compras.API.Services;

/// <summary>
/// PATRÓN DECORATOR: envuelve un IProductoService y le añade logging
/// SIN modificar la clase original (principio Open/Closed de SOLID).
/// Se registra en el contenedor de forma que el controlador recibe el
/// decorador, y el decorador delega en el servicio real.
/// </summary>
public class ProductoServiceLoggingDecorator : IProductoService
{
    private readonly IProductoService _inner;
    private readonly ILogger<ProductoServiceLoggingDecorator> _logger;

    public ProductoServiceLoggingDecorator(
        IProductoService inner,
        ILogger<ProductoServiceLoggingDecorator> logger)
    {
        _inner = inner;
        _logger = logger;
    }

    public async Task<List<ProductoDto>> ListarAsync()
    {
        _logger.LogInformation("[Decorator] Listando productos...");
        var result = await _inner.ListarAsync();
        _logger.LogInformation("[Decorator] {Count} productos devueltos.", result.Count);
        return result;
    }

    public Task<ProductoDto?> ObtenerAsync(int id)
    {
        _logger.LogInformation("[Decorator] Obteniendo producto {Id}", id);
        return _inner.ObtenerAsync(id);
    }

    public async Task<ProductoDto> RegistrarAsync(ProductoCreateDto dto)
    {
        _logger.LogInformation("[Decorator] Registrando producto '{Nombre}'", dto.NombreProducto);
        var creado = await _inner.RegistrarAsync(dto);
        _logger.LogInformation("[Decorator] Producto creado con Id {Id}", creado.IdProducto);
        return creado;
    }

    public Task<ProductoDto?> ActualizarAsync(ProductoUpdateDto dto)
    {
        _logger.LogInformation("[Decorator] Actualizando producto {Id}", dto.IdProducto);
        return _inner.ActualizarAsync(dto);
    }

    public Task<bool> EliminarAsync(int id)
    {
        _logger.LogWarning("[Decorator] Eliminando producto {Id}", id);
        return _inner.EliminarAsync(id);
    }
}
