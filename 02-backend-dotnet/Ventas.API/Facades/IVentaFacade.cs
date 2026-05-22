using Shared.Dtos;

namespace Ventas.API.Facades;

public interface IVentaFacade
{
    Task<int> RegistrarVentaAsync(VentaCreateDto dto);
    Task<List<VentaListDto>> ListarVentasAsync();
}
