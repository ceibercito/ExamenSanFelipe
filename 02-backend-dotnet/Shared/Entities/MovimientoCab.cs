using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Entities;

[Table("MovimientoCab")]
public class MovimientoCab
{
    [Key]
    [Column("Id_MovimientoCab")]
    public int IdMovimientoCab { get; set; }

    [Column("Fec_registro")]
    public DateTime FecRegistro { get; set; } = DateTime.Now;

    /// <summary>1 = Entrada (compra), 2 = Salida (venta)</summary>
    [Column("Id_TipoMovimiento")]
    public int IdTipoMovimiento { get; set; }

    /// <summary>Id_CompraCab o Id_VentaCab segun el tipo de movimiento</summary>
    [Column("Id_DocumentoOrigen")]
    public int IdDocumentoOrigen { get; set; }

    public List<MovimientoDet> Detalles { get; set; } = new();
}
