using Compras.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Common;
using Shared.Dtos;

namespace Compras.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    private readonly IProductoService _service;

    public ProductosController(IProductoService service) => _service = service;

    /// <summary>Listar productos con su stock actual.</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<ProductoDto>>>> Listar()
        => Ok(ApiResponse<List<ProductoDto>>.Ok(await _service.ListarAsync()));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<ProductoDto>>> Obtener(int id)
    {
        var p = await _service.ObtenerAsync(id);
        return p is null
            ? NotFound(ApiResponse<ProductoDto>.Fail("Producto no encontrado."))
            : Ok(ApiResponse<ProductoDto>.Ok(p));
    }

    /// <summary>Registrar un nuevo producto (usado por el modal del frontend).</summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ProductoDto>>> Registrar([FromBody] ProductoCreateDto dto)
        => Ok(ApiResponse<ProductoDto>.Ok(await _service.RegistrarAsync(dto), "Producto registrado."));

    /// <summary>Actualizar un producto existente.</summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<ProductoDto>>> Actualizar(int id, [FromBody] ProductoUpdateDto dto)
    {
        if (id != dto.IdProducto)
            return BadRequest(ApiResponse<ProductoDto>.Fail("El id de la ruta no coincide con el del cuerpo."));

        var actualizado = await _service.ActualizarAsync(dto);
        return actualizado is null
            ? NotFound(ApiResponse<ProductoDto>.Fail("Producto no encontrado."))
            : Ok(ApiResponse<ProductoDto>.Ok(actualizado, "Producto actualizado."));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<bool>>> Eliminar(int id)
    {
        var ok = await _service.EliminarAsync(id);
        return ok
            ? Ok(ApiResponse<bool>.Ok(true, "Producto eliminado."))
            : NotFound(ApiResponse<bool>.Fail("Producto no encontrado."));
    }
}
