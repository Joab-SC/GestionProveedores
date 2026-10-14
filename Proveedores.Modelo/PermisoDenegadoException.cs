namespace Proveedores.Modelo
{
    // La sesion activa no tiene el permiso que el metodo exige.
    public class PermisoDenegadoException : ReglaNegocioException
    {
        public PermisoDenegadoException(string mensaje) : base(mensaje)
        {
        }
    }
}
