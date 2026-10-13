using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using Proveedores.Modelo;

namespace Proveedores.Datos
{
    // CRUD de la tabla CIUDAD. Todo el SQL de esta entidad vive aquí.
    public class CiudadDAO
    {
        public List<Ciudad> Listar()
        {
            var lista = new List<Ciudad>();
            const string sql = "SELECT id, nombre FROM ciudad ORDER BY nombre";

            using (var con = Conexion.Crear())
            using (var cmd = new OracleCommand(sql, con))
            {
                con.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        lista.Add(new Ciudad
                        {
                            Id = rd.GetInt32(0),
                            Nombre = rd.GetString(1)
                        });
                    }
                }
            }
            return lista;
        }

        public int Insertar(Ciudad ciudad)
        {
            const string sql = "INSERT INTO ciudad (nombre) VALUES (:nombre)";

            using (var con = Conexion.Crear())
            using (var cmd = new OracleCommand(sql, con))
            {
                cmd.BindByName = true;
                cmd.Parameters.Add("nombre", OracleDbType.Varchar2).Value = ciudad.Nombre;
                con.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public int Actualizar(Ciudad ciudad)
        {
            const string sql = "UPDATE ciudad SET nombre = :nombre WHERE id = :id";

            using (var con = Conexion.Crear())
            using (var cmd = new OracleCommand(sql, con))
            {
                cmd.BindByName = true;
                cmd.Parameters.Add("nombre", OracleDbType.Varchar2).Value = ciudad.Nombre;
                cmd.Parameters.Add("id", OracleDbType.Int32).Value = ciudad.Id;
                con.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public int Eliminar(int id)
        {
            const string sql = "DELETE FROM ciudad WHERE id = :id";

            using (var con = Conexion.Crear())
            using (var cmd = new OracleCommand(sql, con))
            {
                cmd.BindByName = true;
                cmd.Parameters.Add("id", OracleDbType.Int32).Value = id;
                con.Open();
                return cmd.ExecuteNonQuery();
            }
        }
    }
}