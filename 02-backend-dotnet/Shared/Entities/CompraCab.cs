using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Entities;

[Table("CompraCab")]
public class CompraCab
{
    [Key]
    [Column("Id_CompraCab")]
    public int IdCompraCab { get; set; }

    [Column("FecRegistro")]
    public DateTime FecRegistro { get; set; } = DateTime.Now;

    [Column("SubTotal")]
    public decimal SubTotal { get; set; }

    [Column("Igv")]
    public decimal Igv { get; set; }

    [Column("Total")]
    public decimal Total { get; set; }

    public List<CompraDet> Detalles { get; set; } = new();
}
