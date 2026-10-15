using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using Proveedores.Modelo;

namespace Proveedores.Datos
{
    public class ProductoDAO
    {
        public List<ProductoVistaDto> ListarVista()
        {
            var lista = new List<ProductoVistaDto>();
            const string sql =
                "SELECT p.id, p.nombre, p.descripcion, p.categoria_id, c.nombre AS categoria_nombre " +
                "FROM producto p " +
                "JOIN categoria_producto c ON c.id = p.categoria_id " +
                "ORDER BY p.nombre";

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
                            lista.Add(new ProductoVistaDto
                            {
                                Id = rd.GetInt32(0),
                                Nombre = rd.GetString(1),
                                Descripcion = rd.IsDBNull(2) ? string.Empty : rd.GetString(2),
                                CategoriaId = rd.GetInt32(3),
                                CategoriaNombre = rd.GetString(4)
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

        public List<Producto> Listar()
        {
            var lista = new List<Producto>();
            const string sql = "SELECT id, nombre, descripcion, categoria_id FROM producto ORDER BY nombre";

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
                            lista.Add(new Producto
                            {
                                Id = rd.GetInt32(0),
                                Nombre = rd.GetString(1),
                                Descripcion = rd.IsDBNull(2) ? string.Empty : rd.GetString(2),
                                CategoriaId = rd.GetInt32(3)
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

        public Producto? ObtenerPorId(int id)
        {
            const string sql = "SELECT id, nombre, descripcion, categoria_id FROM producto WHERE id = :id";

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
                            return new Producto
                            {
                                Id = rd.GetInt32(0),
                                Nombre = rd.GetString(1),
                                Descripcion = rd.IsDBNull(2) ? string.Empty : rd.GetString(2),
                                CategoriaId = rd.GetInt32(3)
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

        public int Insertar(Producto producto)
        {
            const string sql =
                "INSERT INTO producto (nombre, descripcion, categoria_id) VALUES (:nombre, :descripcion, :categoria_id)";

            try
            {
                using (var con = Conexion.Crear())
                using (var cmd = new OracleCommand(sql, con))
                {
                    cmd.BindByName = true;
                    cmd.Parameters.Add("nombre", OracleDbType.Varchar2).Value = producto.Nombre;
                    cmd.Parameters.Add("descripcion", OracleDbType.Varchar2).Value =
                        string.IsNullOrWhiteSpace(producto.Descripcion) ? (object)DBNull.Value : producto.Descripcion;
                    cmd.Parameters.Add("categoria_id", OracleDbType.Int32).Value = producto.CategoriaId;
                    con.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
            catch (OracleException ex)
            {
                throw TraductorErroresOracle.Traducir(ex);
            }
        }

        public int Actualizar(Producto producto)
        {
            const string sql =
                "UPDATE producto SET nombre = :nombre, descripcion = :descripcion, categoria_id = :categoria_id WHERE id = :id";

            try
            {
                using (var con = Conexion.Crear())
                using (var cmd = new OracleCommand(sql, con))
                {
                    cmd.BindByName = true;
                    cmd.Parameters.Add("nombre", OracleDbType.Varchar2).Value = producto.Nombre;
                    cmd.Parameters.Add("descripcion", OracleDbType.Varchar2).Value =
                        string.IsNullOrWhiteSpace(producto.Descripcion) ? (object)DBNull.Value : producto.Descripcion;
                    cmd.Parameters.Add("categoria_id", OracleDbType.Int32).Value = producto.CategoriaId;
                    cmd.Parameters.Add("id", OracleDbType.Int32).Value = producto.Id;
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
            const string sql = "DELETE FROM producto WHERE id = :id";

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
