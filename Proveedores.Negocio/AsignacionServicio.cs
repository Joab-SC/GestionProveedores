using System;
using System.Collections.Generic;
using System.Text;
using Proveedores.Datos;
using Proveedores.Modelo;

namespace Proveedores.Negocio;

public class AsignacionServicio
{
    private readonly DetalleProductoDAO _detalleDao = new();
    private readonly ProveedorDAO _proveedorDao = new();

    public List<int> ListarProductoIdsDeProveedor(string cedula)
    {
        Sesion.Exigir(Permisos.Leer);
        return _detalleDao.ListarProductoIdsDeProveedor(cedula);
    }

    public void Guardar(string cedula, List<int> productoIds)
    {
        Sesion.Exigir(Permisos.Crear);
        Sesion.Exigir(Permisos.Actualizar);
        Sesion.Exigir(Permisos.Eliminar);

        if (string.IsNullOrWhiteSpace(cedula))
            throw new ReglaNegocioException("Debe seleccionar un proveedor.");

        if (_proveedorDao.ObtenerPorCedula(cedula) is null)
            throw new ReglaNegocioException("El proveedor no existe.");

        var idsUnicos = productoIds.Distinct().ToList();
        _detalleDao.ReemplazarProductosDeProveedor(cedula, idsUnicos);
    }
}