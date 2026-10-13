using Oracle.ManagedDataAccess.Client;

namespace Proveedores.Datos
{
    public static class Conexion
    {
        private const string Cadena =
        "Data Source=192.168.5.214:1521/FREEPDB1;User Id=proyecto_bd;Password=proyecto123;Connection Timeout=60;";
        public static OracleConnection Crear()
        {
            return new OracleConnection(Cadena);
        }
    }
}