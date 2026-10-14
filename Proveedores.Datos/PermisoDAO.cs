using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using Proveedores.Modelo;

namespace Proveedores.Datos
{
    // Permisos de un tipo de usuario (detalle_tipo_usuario -> permiso).
    public class PermisoDAO
    {
        public List<Permiso> ListarPorTipoUsuario(int tipoUsuarioId)
        {
            var lista = new List<Permiso>();
            const string sql =
                "SELECT p.id, p.nombre " +
                "FROM permiso p " +
                "JOIN detalle_tipo_usuario dtu ON dtu.permiso_id = p.id " +
                "WHERE dtu.tipo_usuario_id = :tipo " +
                "ORDER BY p.id";

            try
            {
                using (var con = Conexion.Crear())
                using (var cmd = new OracleCommand(sql, con))
                {
                    cmd.BindByName = true;
                    cmd.Parameters.Add("tipo", OracleDbType.Int32).Value = tipoUsuarioId;
                    con.Open();

                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            lista.Add(new Permiso
                            {
                                Id = Convert.ToInt32(rd.GetValue(0)),
                                Nombre = rd.GetString(1)
                            });
                        }
                    }
                }
            }
            catch (OracleException ex)
            {
                throw TraductorErroresOracle.Traducir(ex);
            }

            return lista;
        }
    }
}
