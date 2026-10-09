namespace UsuariosApi.DTOs;


public class UsuarioDto
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Apellido { get; set; } = string.Empty;

    public string Correo { get; set; } = string.Empty;

    public string FechaNacimiento { get; set; } = string.Empty;

  
    public int Edad { get; set; }

    public string Universidad { get; set; } = string.Empty;

    public int Semestre { get; set; }

    public bool Activo { get; set; } = true;

    public DateTime FechaCreacion { get; set; }
}
