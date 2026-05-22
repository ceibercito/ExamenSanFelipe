using Shared.Dtos;

namespace Compras.API.Facades;

public interface ICompraFacade
{
    Task<int> RegistrarCompraAsync(CompraCreateDto dto);
    Task<List<CompraListDto>> ListarComprasAsync();
}
