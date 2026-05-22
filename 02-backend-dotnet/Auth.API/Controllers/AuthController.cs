using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Common;
using Shared.Data;
using Shared.Dtos;
using Shared.Security;

namespace Auth.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IJwtService _jwt;

    public AuthController(AppDbContext db, IJwtService jwt)
    {
        _db = db;
        _jwt = jwt;
    }

    /// <summary>Login: valida usuario/contraseña y devuelve un JWT (válido 30 min).</summary>
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest req)
    {
        var usuario = await _db.Usuarios
            .FirstOrDefaultAsync(u => u.Username == req.Username && u.Activo);

        if (usuario is null || !BCrypt.Net.BCrypt.Verify(req.Password, usuario.PasswordHash))
            return Unauthorized(ApiResponse<LoginResponse>.Fail("Usuario o contraseña incorrectos."));

        var (token, expiraEn) = _jwt.GenerarToken(usuario);

        return Ok(ApiResponse<LoginResponse>.Ok(
            new LoginResponse(token, usuario.Username, usuario.Nombre, expiraEn)));
    }
}
