using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace Proveedores.Datos
{
    // Consultas de los 5 reportes. Cada una devuelve un DataTable ya filtrado.
    // Los alias entre comillas dobles son los títulos de las columnas del PDF.
    public class ReporteDAO
    {
        // Reporte 1: proveedores por ciudad, con sus productos (ciudad opcional)
        public DataTable ProveedoresPorCiudad(int? ciudadId)
        {
            const string sql =
                "SELECT c.nombre AS \"Ciudad\", pr.cedula AS \"Cédula\", " +
                "       pr.nombre || ' ' || pr.apellido AS \"Proveedor\", p.nombre AS \"Producto\" " +
                "FROM proveedor pr " +
                "JOIN ciudad c ON c.id = pr.ciudad_id " +
                "LEFT JOIN detalle_producto dp ON dp.proveedor_cedula = pr.cedula " +
                "LEFT JOIN producto p ON p.id = dp.producto_id " +
                "WHERE (:ciudad_id IS NULL OR pr.ciudad_id = :ciudad_id) " +
                "ORDER BY c.nombre, pr.apellido, pr.nombre, p.nombre";

            return Ejecutar(sql, cmd =>
            {
                cmd.Parameters.Add("ciudad_id", OracleDbType.Int32).Value = ValorOpcional(ciudadId);
            });
        }

        // Reporte 2: productos que ofrece un proveedor
        public DataTable ProductosDeProveedor(string cedula)
        {
            const string sql =
                "SELECT pr.cedula AS \"Cédula\", pr.nombre || ' ' || pr.apellido AS \"Proveedor\", " +
                "       p.nombre AS \"Producto\", cat.nombre AS \"Categoría\" " +
                "FROM proveedor pr " +
                "JOIN detalle_producto dp ON dp.proveedor_cedula = pr.cedula " +
                "JOIN producto p ON p.id = dp.producto_id " +
                "JOIN categoria_producto cat ON cat.id = p.categoria_id " +
                "WHERE pr.cedula = :cedula " +
                "ORDER BY p.nombre";

            return Ejecutar(sql, cmd =>
            {
                cmd.Parameters.Add("cedula", OracleDbType.Varchar2).Value = cedula;
            });
        }

        // Reporte 3: proveedores de un producto
        public DataTable ProveedoresDeProducto(int productoId)
        {
            const string sql =
                "SELECT p.nombre AS \"Producto\", pr.cedula AS \"Cédula\", " +
                "       pr.nombre || ' ' || pr.apellido AS \"Proveedor\", c.nombre AS \"Ciudad\" " +
                "FROM producto p " +
                "JOIN detalle_producto dp ON dp.producto_id = p.id " +
                "JOIN proveedor pr ON pr.cedula = dp.proveedor_cedula " +
                "JOIN ciudad c ON c.id = pr.ciudad_id " +
                "WHERE p.id = :producto_id " +
                "ORDER BY pr.apellido, pr.nombre";

            return Ejecutar(sql, cmd =>
            {
                cmd.Parameters.Add("producto_id", OracleDbType.Int32).Value = productoId;
            });
        }

        // Reporte 4: productos por categoría (categoría opcional)
        public DataTable ProductosPorCategoria(int? categoriaId)
        {
            const string sql =
                "SELECT cat.nombre AS \"Categoría\", p.nombre AS \"Producto\", p.descripcion AS \"Descripción\" " +
                "FROM categoria_producto cat " +
                "JOIN producto p ON p.categoria_id = cat.id " +
                "WHERE (:categoria_id IS NULL OR cat.id = :categoria_id) " +
                "ORDER BY cat.nombre, p.nombre";

            return Ejecutar(sql, cmd =>
            {
                cmd.Parameters.Add("categoria_id", OracleDbType.Int32).Value = ValorOpcional(categoriaId);
            });
        }

        // Reporte 5: productos con su categoría, buscados por nombre (categoría opcional)
        public DataTable ProductosBuscados(string texto, int? categoriaId)
        {
            const string sql =
                "SELECT p.nombre AS \"Producto\", p.descripcion AS \"Descripción\", cat.nombre AS \"Categoría\" " +
                "FROM producto p " +
                "JOIN categoria_producto cat ON cat.id = p.categoria_id " +
                "WHERE UPPER(p.nombre) LIKE '%' || UPPER(:texto) || '%' " +
                "  AND (:categoria_id IS NULL OR cat.id = :categoria_id) " +
                "ORDER BY p.nombre";

            return Ejecutar(sql, cmd =>
            {
                cmd.Parameters.Add("texto", OracleDbType.Varchar2).Value = texto;
                cmd.Parameters.Add("categoria_id", OracleDbType.Int32).Value = ValorOpcional(categoriaId);
            });
        }

        private static object ValorOpcional(int? valor)
        {
            return valor.HasValue ? valor.Value : DBNull.Value;
        }

        // Ejecuta el SQL con sus parámetros y devuelve el resultado como tabla.
        private static DataTable Ejecutar(string sql, Action<OracleCommand> agregarParametros)
        {
            var tabla = new DataTable();

            try
            {
                using (var con = Conexion.Crear())
                using (var cmd = new OracleCommand(sql, con))
                {
                    cmd.BindByName = true;
                    agregarParametros(cmd);

                    using (var adaptador = new OracleDataAdapter(cmd))
                    {
                        adaptador.Fill(tabla);   // abre y cierra la conexión por sí solo
                    }
                }
            }
            catch (OracleException ex)
            {
                throw TraductorErroresOracle.Traducir(ex);
            }

            return tabla;
        }
    }
}