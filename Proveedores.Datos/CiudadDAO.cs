using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using Proveedores.Modelo;

namespace Proveedores.Datos
{
    // CRUD de la tabla CIUDAD. Todo el SQL de esta entidad vive aquí.
    // Cada método traduce su OracleException con TraductorErroresOracle,
    // así ninguna capa superior ve un texto "ORA-xxxxx".
    public class CiudadDAO
    {
        public List<Ciudad> Listar()
        {
            var lista = new List<Ciudad>();
            const string sql = "SELECT id, nombre FROM ciudad ORDER BY nombre";

            try
            {
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
            }
            catch (OracleException ex)
            {
                throw TraductorErroresOracle.Traducir(ex);
            }

            return lista;
        }

        public int Insertar(Ciudad ciudad)
        {
            const string sql = "INSERT INTO ciudad (nombre) VALUES (:nombre)";

            try
            {
                using (var con = Conexion.Crear())
                using (var cmd = new OracleCommand(sql, con))
                {
                    cmd.BindByName = true;
                    cmd.Parameters.Add("nombre", OracleDbType.Varchar2).Value = ciudad.Nombre;
                    con.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
            catch (OracleException ex)
            {
                throw TraductorErroresOracle.Traducir(ex);
            }
        }

        public int Actualizar(Ciudad ciudad)
        {
            const string sql = "UPDATE ciudad SET nombre = :nombre WHERE id = :id";

            try
            {
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
            catch (OracleException ex)
            {
                throw TraductorErroresOracle.Traducir(ex);
            }
        }

        public int Eliminar(int id)
        {
            const string sql = "DELETE FROM ciudad WHERE id = :id";

            try
            {
                using (var con = Conexion.Crear())
                using (var cmd = new OracleCommand(sql, con))
                {
                    cmd.BindByName = true;
                    cmd.Parameters.Add("id", OracleDbType.Int32).Value = id;
                    con.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
            catch (OracleException ex)
            {
                throw TraductorErroresOracle.Traducir(ex);
            }
        }
    }
}