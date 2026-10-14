using System;
using Oracle.ManagedDataAccess.Client;
using Proveedores.Modelo;

namespace Proveedores.Datos
{
    // Lecturas de la tabla USUARIO. Todo el SQL vive aquí.
    // Las traducciones de Oracle las hace TraductorErroresOracle.
    public class UsuarioDAO
    {
        public Usuario? ObtenerPorCedula(string cedula)
        {
            const string sql =
                "SELECT cedula, nombre, apellido, fecha_nacimiento, ciudad_id, " +
                "tipo_usuario_id, contrasena, activo " +
                "FROM usuario WHERE cedula = :cedula";

            try
            {
                using (var con = Conexion.Crear())
                using (var cmd = new OracleCommand(sql, con))
                {
                    cmd.BindByName = true;
                    cmd.Parameters.Add("cedula", OracleDbType.Varchar2).Value = cedula;
                    con.Open();

                    using (var rd = cmd.ExecuteReader())
                    {
                        if (!rd.Read())
                        {
                            return null;   // no existe esa cedula
                        }

                        return new Usuario
                        {
                            Cedula = rd.GetString(0),
                            Nombre = rd.GetString(1),
                            Apellido = rd.GetString(2),
                            FechaNacimiento = rd.GetDateTime(3),
                            CiudadId = Convert.ToInt32(rd.GetValue(4)),
                            TipoUsuarioId = Convert.ToInt32(rd.GetValue(5)),
                            Contrasena = rd.GetString(6),
                            Activo = Convert.ToInt32(rd.GetValue(7)) != 0
                        };
                    }
                }
            }
            catch (OracleException ex)
            {
                throw TraductorErroresOracle.Traducir(ex);
            }
        }
    }
}
