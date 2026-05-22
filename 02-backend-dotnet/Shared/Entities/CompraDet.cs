using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Entities;

[Table("CompraDet")]
public class CompraDet
{
    [Key]
    [Column("Id_CompraDet")]
    public int IdCompraDet { get; set; }

    [Column("Id_CompraCab")]
    public int IdCompraCab { get; set; }

    [Column("Id_producto")]
    public int IdProducto { get; set; }

    [Column("Cantidad")]
    public int Cantidad { get; set; }

    [Column("Precio")]
    public decimal Precio { get; set; }

    [Column("Sub_Total")]
    public decimal SubTotal { get; set; }

    [Column("Igv")]
    public decimal Igv { get; set; }

    [Column("Total")]
    public decimal Total { get; set; }

    [ForeignKey(nameof(IdProducto))]
    public Producto? Producto { get; set; }
}
