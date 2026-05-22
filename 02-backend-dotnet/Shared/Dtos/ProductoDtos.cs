namespace Shared.Dtos;

public record ProductoCreateDto(string NombreProducto, string? NroLote, decimal Costo, decimal PrecioVenta);

public record ProductoUpdateDto(int IdProducto, string NombreProducto, string? NroLote, decimal Costo, decimal PrecioVenta);

public record ProductoDto(
    int IdProducto,
    string NombreProducto,
    string? NroLote,
    decimal Costo,
    decimal PrecioVenta,
    int StockActual);
