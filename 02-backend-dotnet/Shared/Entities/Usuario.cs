using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Entities;

[Table("Usuarios")]
public class Usuario
{
    [Key]
    [Column("Id_Usuario")]
    public int Id { get; set; }

    [Column("Username")]
    public string Username { get; set; } = string.Empty;

    [Column("PasswordHash")]
    public string PasswordHash { get; set; } = string.Empty;

    [Column("Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Column("Activo")]
    public bool Activo { get; set; } = true;

    [Column("Fec_registro")]
    public DateTime FecRegistro { get; set; } = DateTime.Now;
}
