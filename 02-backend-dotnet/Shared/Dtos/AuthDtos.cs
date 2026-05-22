namespace Shared.Dtos;

public record LoginRequest(string Username, string Password);

public record LoginResponse(string Token, string Username, string Nombre, DateTime ExpiraEn);
