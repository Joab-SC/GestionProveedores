namespace Proveedores.Modelo
{
    public class Proveedor
    {
        public string Cedula { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public int CiudadId { get; set; }

        public string NombreCompleto => $"{Nombre} {Apellido}";

        // Para mostrar al proveedor con su nombre en ComboBox y listas (por ejemplo, en las asignaciones)
        public override string ToString()
        {
            return NombreCompleto;
        }
    }
}