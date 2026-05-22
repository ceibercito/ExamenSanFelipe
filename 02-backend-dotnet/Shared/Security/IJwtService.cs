using Shared.Entities;

namespace Shared.Security;

public interface IJwtService
{
    /// <summary>Genera un JWT para el usuario y devuelve el token y su fecha de expiración.</summary>
    (string token, DateTime expiraEn) GenerarToken(Usuario usuario);
}
