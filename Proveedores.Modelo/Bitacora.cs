namespace Proveedores.Modelo
{
    // Fila de la tabla BITACORA. FechaSalida queda en null mientras la
    // sesion sigue abierta (su registro lo hace el CU-02).
    public class Bitacora
    {
        public int Id { get; set; }
        public string UsuarioCedula { get; set; } = string.Empty;
        public DateTime FechaEntrada { get; set; }
        public DateTime? FechaSalida { get; set; }
    }
}
