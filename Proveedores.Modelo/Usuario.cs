namespace Proveedores.Modelo
{
    // Entidad USUARIO. La contrasena nunca se guarda en texto plano:
    // solo el hash SHA-256 (64 caracteres en hexadecimal).
    public class Usuario
    {
        public string Cedula { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public int CiudadId { get; set; }
        public int TipoUsuarioId { get; set; }
        public string Contrasena { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;
    }
}
