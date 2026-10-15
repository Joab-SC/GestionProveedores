namespace Proveedores.Modelo
{
    // Fila de proveedor ya unida con el nombre de su ciudad, solo para mostrar en tablas.
    public class ProveedorVistaDto
    {
        public string Cedula { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Ciudad { get; set; } = string.Empty;
    }
}