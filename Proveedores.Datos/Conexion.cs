using Oracle.ManagedDataAccess.Client;

using DotNetEnv;

namespace Proveedores.Datos
{
    public static class Conexion
    {
        private static readonly Lazy<ConfiguracionConexion> configuracion = new(CargarConfiguracion);

        public static OracleConnection Crear()
        {
            var valores = configuracion.Value;

            string cadena =
                "Data Source=" + valores.Host + ":" + valores.Puerto + "/" + valores.Servicio +
                ";User Id=" + valores.Usuario +
                ";Password=" + valores.Clave +
                ";Connection Timeout=60;";

            // Se devuelve SIN abrir: el DAO decide cuándo abrirla (con using).
            return new OracleConnection(cadena);
        }

        private static ConfiguracionConexion CargarConfiguracion()
        {
            string? rutaEnv = BuscarArchivoEnv();
            if (rutaEnv != null)
                Env.Load(rutaEnv, new LoadOptions(true, false, true));

            return new ConfiguracionConexion(
                Requerir("ORACLE_HOST"),
                Requerir("ORACLE_PORT"),
                Requerir("ORACLE_SERVICE"),
                Requerir("APP_USER"),
                Requerir("APP_USER_PASSWORD"));
        }

        private static string Requerir(string variable)
        {
            string? valor = Environment.GetEnvironmentVariable(variable);
            if (string.IsNullOrWhiteSpace(valor))
                throw new InvalidOperationException(
                    $"Falta la variable de entorno obligatoria '{variable}' para la conexión a Oracle.");

            return valor.Trim();
        }

        private static string? BuscarArchivoEnv()
        {
            for (DirectoryInfo? directorio = new(AppContext.BaseDirectory);
                 directorio != null;
                 directorio = directorio.Parent)
            {
                string rutaEnv = Path.Combine(directorio.FullName, ".env");
                if (File.Exists(rutaEnv))
                    return rutaEnv;

                string rutaEnvEnDatabase = Path.Combine(directorio.FullName, "Database", ".env");
                if (File.Exists(rutaEnvEnDatabase))
                    return rutaEnvEnDatabase;
            }

            return null;
        }

        private sealed record ConfiguracionConexion(
            string Host,
            string Puerto,
            string Servicio,
            string Usuario,
            string Clave);
    }
}
