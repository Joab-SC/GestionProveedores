using Oracle.ManagedDataAccess.Client;
using Proveedores.Modelo;

namespace Proveedores.Datos;

public class ConsultaDAO
{
    public List<ProveedorProductoDto> ProveedoresConCiudadYProductos() => Listar("""
        SELECT pr.cedula, pr.nombre || ' ' || pr.apellido AS proveedor,
               c.nombre AS ciudad, p.nombre AS producto
        FROM proveedor pr
        JOIN ciudad c ON c.id = pr.ciudad_id
        JOIN detalle_producto dp ON dp.proveedor_cedula = pr.cedula
        JOIN producto p ON p.id = dp.producto_id
        ORDER BY proveedor, producto
        """,
        r => new ProveedorProductoDto(r.GetString(0), r.GetString(1), r.GetString(2), r.GetString(3)));

    public List<ProductoPorCategoriaDto> ProductosPorCategoriaConProveedores() => Listar("""
        SELECT cat.nombre AS categoria, p.nombre AS producto, COUNT(dp.id) AS proveedores
        FROM categoria_producto cat
        JOIN producto p ON p.categoria_id = cat.id
        LEFT JOIN detalle_producto dp ON dp.producto_id = p.id
        GROUP BY cat.nombre, p.nombre
        ORDER BY cat.nombre, p.nombre
        """,
        r => new ProductoPorCategoriaDto(r.GetString(0), r.GetString(1), r.GetInt32(2)));

    public List<UsuarioPermisoDto> UsuariosConCiudadTipoYPermisos() => Listar("""
        SELECT u.cedula, u.nombre || ' ' || u.apellido AS usuario, c.nombre AS ciudad,
               t.nombre AS tipo, pe.nombre AS permiso
        FROM usuario u
        JOIN ciudad c ON c.id = u.ciudad_id
        JOIN tipo_usuario t ON t.id = u.tipo_usuario_id
        JOIN detalle_tipo_usuario dtu ON dtu.tipo_usuario_id = t.id
        JOIN permiso pe ON pe.id = dtu.permiso_id
        ORDER BY usuario, permiso
        """,
        r => new UsuarioPermisoDto(r.GetString(0), r.GetString(1), r.GetString(2), r.GetString(3), r.GetString(4)));

    public List<ProveedorResumenDto> ProveedoresConMasProductosQuePromedio() => Listar("""
        SELECT pr.cedula, pr.nombre, pr.apellido
        FROM proveedor pr
        WHERE (SELECT COUNT(*) FROM detalle_producto dp
               WHERE dp.proveedor_cedula = pr.cedula)
            > (SELECT AVG(n) FROM (SELECT COUNT(*) AS n FROM detalle_producto
                                   GROUP BY proveedor_cedula))
        """,
        r => new ProveedorResumenDto(r.GetString(0), r.GetString(1), r.GetString(2)));

    public List<ProductoSinProveedorDto> ProductosSinProveedor() => Listar("""
        SELECT p.id, p.nombre, p.descripcion
        FROM producto p
        WHERE NOT EXISTS (SELECT 1 FROM detalle_producto dp WHERE dp.producto_id = p.id)
        """,
        r => new ProductoSinProveedorDto(
            r.GetInt32(0),
            r.GetString(1),
            r.IsDBNull(2) ? null : r.GetString(2)));

    private static List<T> Listar<T>(string sql, Func<OracleDataReader, T> mapear)
    {
        try
        {
            using var con = Conexion.Crear();
            con.Open();
            using var cmd = con.CreateCommand();
            cmd.BindByName = true;
            cmd.CommandText = sql;
            using var lector = cmd.ExecuteReader();

            var resultado = new List<T>();
            while (lector.Read())
                resultado.Add(mapear(lector));
            return resultado;
        }
        catch (OracleException ex)
        {
            throw TraductorErroresOracle.Traducir(ex);
        }
    }
}