using Proveedores.Datos;
using Proveedores.Modelo;

namespace Proveedores.Negocio;

public class ConsultaServicio
{
    private readonly ConsultaDAO dao = new();

    public List<ProveedorProductoDto> ProveedoresConCiudadYProductos()
    {
        Sesion.Exigir(Permisos.Leer);
        return dao.ProveedoresConCiudadYProductos();
    }

    public List<ProductoPorCategoriaDto> ProductosPorCategoriaConProveedores()
    {
        Sesion.Exigir(Permisos.Leer);
        return dao.ProductosPorCategoriaConProveedores();
    }

    public List<UsuarioPermisoDto> UsuariosConCiudadTipoYPermisos()
    {
        Sesion.Exigir(Permisos.Leer);
        return dao.UsuariosConCiudadTipoYPermisos();
    }

    public List<ProveedorResumenDto> ProveedoresConMasProductosQuePromedio()
    {
        Sesion.Exigir(Permisos.Leer);
        return dao.ProveedoresConMasProductosQuePromedio();
    }

    public List<ProductoSinProveedorDto> ProductosSinProveedor()
    {
        Sesion.Exigir(Permisos.Leer);
        return dao.ProductosSinProveedor();
    }
}