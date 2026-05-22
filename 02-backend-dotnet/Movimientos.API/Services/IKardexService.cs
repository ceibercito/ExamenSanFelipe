using Shared.Dtos;

namespace Movimientos.API.Services;

public interface IKardexService
{
    Task<List<KardexDto>> ListarKardexAsync();
    Task<List<MovimientoDto>> ListarMovimientosProductoAsync(int idProducto);
}
