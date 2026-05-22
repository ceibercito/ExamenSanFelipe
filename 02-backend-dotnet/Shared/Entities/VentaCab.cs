using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Entities;

[Table("VentaCab")]
public class VentaCab
{
    [Key]
    [Column("Id_VentaCab")]
    public int IdVentaCab { get; set; }

    [Column("fecRegistro")]
    public DateTime FecRegistro { get; set; } = DateTime.Now;

    [Column("SubTotal")]
    public decimal SubTotal { get; set; }

    [Column("Igv")]
    public decimal Igv { get; set; }

    [Column("Total")]
    public decimal Total { get; set; }

    public List<VentaDet> Detalles { get; set; } = new();
}
