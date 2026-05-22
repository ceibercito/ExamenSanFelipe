using Compras.API.Facades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Common;
using Shared.Dtos;

namespace Compras.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ComprasController : ControllerBase
{
    private readonly ICompraFacade _facade;

    public ComprasController(ICompraFacade facade) => _facade = facade;

    /// <summary>
    /// Registrar una compra (varios productos). Internamente la fachada:
    /// inserta CompraCab/Det, actualiza el producto y crea el movimiento de Entrada.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<int>>> Registrar([FromBody] CompraCreateDto dto)
    {
        try
        {
            var id = await _facade.RegistrarCompraAsync(dto);
            return Ok(ApiResponse<int>.Ok(id, "Compra registrada correctamente."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<int>.Fail(ex.Message));
        }
    }

    /// <summary>Listar compras (cabeceras).</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<CompraListDto>>>> Listar()
        => Ok(ApiResponse<List<CompraListDto>>.Ok(await _facade.ListarComprasAsync()));
}
