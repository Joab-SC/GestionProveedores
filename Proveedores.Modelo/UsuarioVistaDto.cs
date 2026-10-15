namespace Proveedores.Modelo;

public class UsuarioVistaDto
{
    public string Cedula { get; set; } = "";
    public string NombreCompleto { get; set; } = "";
    public string Ciudad { get; set; } = "";
    public string Tipo { get; set; } = "";
    public bool Activo { get; set; }
}