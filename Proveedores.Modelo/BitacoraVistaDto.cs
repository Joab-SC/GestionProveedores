

namespace Proveedores.Modelo;

public class BitacoraVistaDto
{
    public string Usuario { get; set; } = string.Empty;
    public DateTime FechaEntrada { get; set; }
    public DateTime? FechaSalida { get; set; }   // null = sesión sin cierre
}