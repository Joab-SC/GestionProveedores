using Proveedores.Datos;
using Proveedores.Modelo;

namespace Proveedores.Negocio;

public class ConsultaServicio
{
    private readonly ConsultaDAO dao = new();

    public List<ProveedorProductoDto> ObtenerProveedoresConCiudadYProductos()
    {
        Sesion.Exigir(Permisos.Leer);
        return dao.ProveedoresConCiudadYProductos();
    }

    public List<ProductoPorCategoriaDto> ObtenerProductosPorCategoriaConProveedores()
    {
        Sesion.Exigir(Permisos.Leer);
        return dao.ProductosPorCategoriaConProveedores();
    }

    public List<UsuarioPermisoDto> ObtenerUsuariosConCiudadTipoYPermisos()
    {
        Sesion.Exigir(Permisos.Leer);
        return dao.UsuariosConCiudadTipoYPermisos();
    }

    public List<ProveedorResumenDto> ObtenerProveedoresConMasProductosQuePromedio()
    {
        Sesion.Exigir(Permisos.Leer);
        return dao.ProveedoresConMasProductosQuePromedio();
    }

    public List<ProductoSinProveedorDto> ObtenerProductosSinProveedor()
    {
        Sesion.Exigir(Permisos.Leer);
        return dao.ProductosSinProveedor();
    }
}