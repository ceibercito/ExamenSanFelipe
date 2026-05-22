using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Entities;

[Table("MovimientoDet")]
public class MovimientoDet
{
    [Key]
    [Column("Id_MovimientoDet")]
    public int IdMovimientoDet { get; set; }

    [Column("Id_movimientocab")]
    public int IdMovimientoCab { get; set; }

    [Column("Id_Producto")]
    public int IdProducto { get; set; }

    [Column("Cantidad")]
    public int Cantidad { get; set; }
}
