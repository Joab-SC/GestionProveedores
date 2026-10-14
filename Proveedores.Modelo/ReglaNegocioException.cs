namespace Proveedores.Modelo
{
    // Base de todas las excepciones de regla del negocio.
    // Las capas superiores solo atrapan esta familia de excepciones.
    public class ReglaNegocioException : Exception
    {
        public ReglaNegocioException(string mensaje) : base(mensaje)
        {
        }
    }
}
