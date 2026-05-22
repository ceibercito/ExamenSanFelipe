namespace Shared.Security;

public class JwtSettings
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    /// <summary>Duración del token en minutos. El examen pide 30.</summary>
    public int DurationMinutes { get; set; } = 30;
}
