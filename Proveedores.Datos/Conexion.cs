using Oracle.ManagedDataAccess.Client;

namespace Proveedores.Datos
{
    public static class Conexion
    {
        // Los valores por defecto son los de Database/compose.yaml (desarrollo).
        // Cada persona puede cambiarlos con variables de entorno sin tocar el código.
        public static OracleConnection Crear()
        {
            string host = Leer("ORACLE_HOST", "localhost");
            string servicio = Leer("ORACLE_SERVICE", "FREEPDB1");
            string usuario = Leer("APP_USER", "Joab");
            string clave = Leer("APP_USER_PASSWORD", "Joab2007");

            string cadena =
                "Data Source=" + host + "/" + servicio +
                ";User Id=" + usuario +
                ";Password=" + clave +
                ";Connection Timeout=60;";

            // Se devuelve SIN abrir: el DAO decide cuándo abrirla (con using).
            return new OracleConnection(cadena);
        }

        private static string Leer(string variable, string valorPorDefecto)
        {
            string? valor = Environment.GetEnvironmentVariable(variable);
            return string.IsNullOrWhiteSpace(valor) ? valorPorDefecto : valor;
        }
    }
}