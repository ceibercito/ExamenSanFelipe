namespace Shared.Dtos;

public record CompraDetItemDto(int IdProducto, int Cantidad, decimal Precio);

public record CompraCreateDto(List<CompraDetItemDto> Detalles);

public record CompraListDto(
    int IdCompraCab,
    DateTime FecRegistro,
    decimal SubTotal,
    decimal Igv,
    decimal Total);
