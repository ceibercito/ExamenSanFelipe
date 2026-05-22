using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Entities;

[Table("Productos")]
public class Producto
{
    [Key]
    [Column("Id_producto")]
    public int IdProducto { get; set; }

    [Column("Nombre_producto")]
    public string NombreProducto { get; set; } = string.Empty;

    [Column("NroLote")]
    public string? NroLote { get; set; }

    [Column("Fec_registro")]
    public DateTime FecRegistro { get; set; } = DateTime.Now;

    [Column("Costo")]
    public decimal Costo { get; set; }

    [Column("PrecioVenta")]
    public decimal PrecioVenta { get; set; }
}
