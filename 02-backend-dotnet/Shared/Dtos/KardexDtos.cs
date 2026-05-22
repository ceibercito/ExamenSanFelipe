namespace Shared.Dtos;

public record KardexDto(
    int IdProducto,
    string NombreProducto,
    int StockActual,
    decimal Costo,
    decimal PrecioVenta);

public record MovimientoDto(
    DateTime FechaRegistro,
    string TipoMovimiento,
    int Cantidad,
    int DocumentoOrigen);
