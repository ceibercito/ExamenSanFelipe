using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Common;
using Shared.Dtos;
using Ventas.API.Facades;

namespace Ventas.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class VentasController : ControllerBase
{
    private readonly IVentaFacade _facade;

    public VentasController(IVentaFacade facade) => _facade = facade;

    /// <summary>Registrar una venta. Valida stock y genera movimiento de Salida.</summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<int>>> Registrar([FromBody] VentaCreateDto dto)
    {
        try
        {
            var id = await _facade.RegistrarVentaAsync(dto);
            return Ok(ApiResponse<int>.Ok(id, "Venta registrada correctamente."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<int>.Fail(ex.Message));
        }
    }

    /// <summary>Listar ventas (cabeceras).</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<VentaListDto>>>> Listar()
        => Ok(ApiResponse<List<VentaListDto>>.Ok(await _facade.ListarVentasAsync()));
}
