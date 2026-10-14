using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using Proveedores.Modelo;

namespace Proveedores.Datos
{
    // CRUD de la tabla CATEGORIA_PRODUCTO. Todo el SQL de esta entidad vive aquí.
    public class CategoriaDAO
    {
        public List<CategoriaProducto> Listar()
        {
            var lista = new List<CategoriaProducto>();
            const string sql = "SELECT id, nombre, descripcion FROM categoria_producto ORDER BY nombre";

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
                            lista.Add(new CategoriaProducto
                            {
                                Id = rd.GetInt32(0),
                                Nombre = rd.GetString(1),
                                Descripcion = rd.IsDBNull(2) ? string.Empty : rd.GetString(2)
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

        public CategoriaProducto? ObtenerPorId(int id)
        {
            const string sql = "SELECT id, nombre, descripcion FROM categoria_producto WHERE id = :id";

            try
            {
                using (var con = Conexion.Crear())
                using (var cmd = new OracleCommand(sql, con))
                {
                    cmd.BindByName = true;
                    cmd.Parameters.Add("id", OracleDbType.Int32).Value = id;
                    con.Open();
                    using (var rd = cmd.ExecuteReader())
                    {
                        if (rd.Read())
                        {
                            return new CategoriaProducto
                            {
                                Id = rd.GetInt32(0),
                                Nombre = rd.GetString(1),
                                Descripcion = rd.IsDBNull(2) ? string.Empty : rd.GetString(2)
                            };
                        }
                    }
                }
            }
            catch (OracleException ex)
            {
                throw TraductorErroresOracle.Traducir(ex);
            }

            return null;
        }

        public int Insertar(CategoriaProducto categoria)
        {
            const string sql =
                "INSERT INTO categoria_producto (nombre, descripcion) VALUES (:nombre, :descripcion)";

            try
            {
                using (var con = Conexion.Crear())
                using (var cmd = new OracleCommand(sql, con))
                {
                    cmd.BindByName = true;
                    cmd.Parameters.Add("nombre", OracleDbType.Varchar2).Value = categoria.Nombre;
                    cmd.Parameters.Add("descripcion", OracleDbType.Varchar2).Value =
                        string.IsNullOrWhiteSpace(categoria.Descripcion) ? (object)DBNull.Value : categoria.Descripcion;
                    con.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
            catch (OracleException ex)
            {
                throw TraductorErroresOracle.Traducir(ex);
            }
        }

        public int Actualizar(CategoriaProducto categoria)
        {
            const string sql =
                "UPDATE categoria_producto SET nombre = :nombre, descripcion = :descripcion WHERE id = :id";

            try
            {
                using (var con = Conexion.Crear())
                using (var cmd = new OracleCommand(sql, con))
                {
                    cmd.BindByName = true;
                    cmd.Parameters.Add("nombre", OracleDbType.Varchar2).Value = categoria.Nombre;
                    cmd.Parameters.Add("descripcion", OracleDbType.Varchar2).Value =
                        string.IsNullOrWhiteSpace(categoria.Descripcion) ? (object)DBNull.Value : categoria.Descripcion;
                    cmd.Parameters.Add("id", OracleDbType.Int32).Value = categoria.Id;
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
            const string sql = "DELETE FROM categoria_producto WHERE id = :id";

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