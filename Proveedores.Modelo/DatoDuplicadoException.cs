namespace Proveedores.Modelo
{
    // ORA-00001: se intento guardar un valor que ya existe (llave o unico).
    public class DatoDuplicadoException : ReglaNegocioException
    {
        public DatoDuplicadoException(string mensaje) : base(mensaje)
        {
        }
    }
}
