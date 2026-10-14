namespace Proveedores.Modelo
{
    // ORA-02292: no se puede borrar porque otros registros dependen de este.
    public class RegistroEnUsoException : ReglaNegocioException
    {
        public RegistroEnUsoException(string mensaje) : base(mensaje)
        {
        }
    }
}
