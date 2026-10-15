using Oracle.ManagedDataAccess.Client;
using Proveedores.Modelo;

namespace Proveedores.Datos;

public class UsuarioDAO
{
    private const string Columnas =
        "cedula, nombre, apellido, fecha_nacimiento, ciudad_id, tipo_usuario_id, contrasena, activo";

    public List<UsuarioVistaDto> ListarVista()
    {
        const string sql = @"
            SELECT u.cedula, u.nombre || ' ' || u.apellido AS nombre_completo,
                   c.nombre AS ciudad, t.nombre AS tipo, u.activo
            FROM usuario u
            JOIN ciudad c ON c.id = u.ciudad_id
            JOIN tipo_usuario t ON t.id = u.tipo_usuario_id
            ORDER BY u.apellido, u.nombre";
        try
        {
            using var con = Conexion.Crear();
            con.Open();
            using var cmd = new OracleCommand(sql, con) { BindByName = true };
            using var rd = cmd.ExecuteReader();

            var lista = new List<UsuarioVistaDto>();
            while (rd.Read())
            {
                lista.Add(new UsuarioVistaDto
                {
                    Cedula = rd.GetString(0),
                    NombreCompleto = rd.GetString(1),
                    Ciudad = rd.GetString(2),
                    Tipo = rd.GetString(3),
                    Activo = Convert.ToInt32(rd.GetValue(4)) == 1
                });
            }
            return lista;
        }
        catch (OracleException ex) { throw TraductorErroresOracle.Traducir(ex); }
    }

    public Usuario? ObtenerPorCedula(string cedula)
    {
        const string sql = "SELECT " + Columnas + " FROM usuario WHERE cedula = :cedula";
        try
        {
            using var con = Conexion.Crear();
            con.Open();
            using var cmd = new OracleCommand(sql, con) { BindByName = true };
            cmd.Parameters.Add("cedula", OracleDbType.Varchar2).Value = cedula;
            using var rd = cmd.ExecuteReader();

            if (!rd.Read()) return null;
            return new Usuario
            {
                Cedula = rd.GetString(0),
                Nombre = rd.GetString(1),
                Apellido = rd.GetString(2),
                FechaNacimiento = rd.GetDateTime(3),
                CiudadId = Convert.ToInt32(rd.GetValue(4)),
                TipoUsuarioId = Convert.ToInt32(rd.GetValue(5)),
                Contrasena = rd.GetString(6),
                Activo = Convert.ToInt32(rd.GetValue(7)) == 1
            };
        }
        catch (OracleException ex) { throw TraductorErroresOracle.Traducir(ex); }
    }

    public void Insertar(Usuario u)
    {
        const string sql = @"
            INSERT INTO usuario (cedula, nombre, apellido, fecha_nacimiento,
                                 ciudad_id, tipo_usuario_id, contrasena, activo)
            VALUES (:cedula, :nombre, :apellido, :fecha, :ciudad, :tipo, :contrasena, :activo)";
        Ejecutar(sql, u);
    }

    public void Actualizar(Usuario u)
    {
        const string sql = @"
            UPDATE usuario
               SET nombre = :nombre, apellido = :apellido, fecha_nacimiento = :fecha,
                   ciudad_id = :ciudad, tipo_usuario_id = :tipo,
                   contrasena = :contrasena, activo = :activo
             WHERE cedula = :cedula";
        Ejecutar(sql, u);
    }

    public void CambiarEstado(string cedula, bool activo)
    {
        const string sql = "UPDATE usuario SET activo = :activo WHERE cedula = :cedula";
        try
        {
            using var con = Conexion.Crear();
            con.Open();
            using var cmd = new OracleCommand(sql, con) { BindByName = true };
            cmd.Parameters.Add("activo", OracleDbType.Int32).Value = activo ? 1 : 0;
            cmd.Parameters.Add("cedula", OracleDbType.Varchar2).Value = cedula;
            cmd.ExecuteNonQuery();
        }
        catch (OracleException ex) { throw TraductorErroresOracle.Traducir(ex); }
    }

    public bool ExisteAdministrador()
    {
        const string sql = "SELECT COUNT(*) FROM usuario WHERE tipo_usuario_id = :tipo";
        try
        {
            using var con = Conexion.Crear();
            con.Open();
            using var cmd = new OracleCommand(sql, con) { BindByName = true };
            cmd.Parameters.Add("tipo", OracleDbType.Int32).Value = TiposUsuario.Administrador;
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }
        catch (OracleException ex) { throw TraductorErroresOracle.Traducir(ex); }
    }

    // Insertar y Actualizar usan los mismos 8 parámetros, así que comparten este método.
    private static void Ejecutar(string sql, Usuario u)
    {
        try
        {
            using var con = Conexion.Crear();
            con.Open();
            using var cmd = new OracleCommand(sql, con) { BindByName = true };
            cmd.Parameters.Add("cedula", OracleDbType.Varchar2).Value = u.Cedula;
            cmd.Parameters.Add("nombre", OracleDbType.Varchar2).Value = u.Nombre;
            cmd.Parameters.Add("apellido", OracleDbType.Varchar2).Value = u.Apellido;
            cmd.Parameters.Add("fecha", OracleDbType.Date).Value = u.FechaNacimiento;
            cmd.Parameters.Add("ciudad", OracleDbType.Int32).Value = u.CiudadId;
            cmd.Parameters.Add("tipo", OracleDbType.Int32).Value = u.TipoUsuarioId;
            cmd.Parameters.Add("contrasena", OracleDbType.Varchar2).Value = u.Contrasena;
            cmd.Parameters.Add("activo", OracleDbType.Int32).Value = u.Activo ? 1 : 0;
            cmd.ExecuteNonQuery();
        }
        catch (OracleException ex) { throw TraductorErroresOracle.Traducir(ex); }
    }
}