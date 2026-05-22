namespace Shared.Dtos;

public record VentaDetItemDto(int IdProducto, int Cantidad);

public record VentaCreateDto(List<VentaDetItemDto> Detalles);

public record VentaListDto(
    int IdVentaCab,
    DateTime FecRegistro,
    decimal SubTotal,
    decimal Igv,
    decimal Total);
