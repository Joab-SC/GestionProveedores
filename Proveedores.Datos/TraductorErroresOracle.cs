using Oracle.ManagedDataAccess.Client;
using Proveedores.Modelo;

namespace Proveedores.Datos
{
    // Traduce los errores de Oracle a las excepciones de negocio.
    // Ningun mensaje con "ORA-xxxxx" llega hasta la pantalla.
    public static class TraductorErroresOracle
    {
        public static Exception Traducir(OracleException ex)
        {
            switch (ex.Number)
            {
                case 1:     // ORA-00001: valor duplicado (llave o unico)
                    return new DatoDuplicadoException(
                        "Ya existe un registro con ese valor.");

                case 2292:  // ORA-02292: hay registros hijos
                    return new RegistroEnUsoException(
                        "No se puede eliminar: otros registros dependen de este.");

                case 2291:  // ORA-02291: la referencia no existe
                    return new ReglaNegocioException("La referencia no existe");

                default:    // cualquier otro error se relanza tal cual
                    return ex;
            }
        }
    }
}
