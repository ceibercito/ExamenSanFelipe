using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movimientos.API.Services;
using Shared.Common;
using Shared.Dtos;

namespace Movimientos.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class KardexController : ControllerBase
{
    private readonly IKardexService _service;

    public KardexController(IKardexService service) => _service = service;

    /// <summary>Listar Kardex: cada producto con stock actual, costo y precio de venta.</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<KardexDto>>>> Listar()
        => Ok(ApiResponse<List<KardexDto>>.Ok(await _service.ListarKardexAsync()));

    /// <summary>Detalle de movimientos (entradas/salidas) de un producto.</summary>
    [HttpGet("{idProducto:int}/movimientos")]
    public async Task<ActionResult<ApiResponse<List<MovimientoDto>>>> Movimientos(int idProducto)
        => Ok(ApiResponse<List<MovimientoDto>>.Ok(
            await _service.ListarMovimientosProductoAsync(idProducto)));
}
