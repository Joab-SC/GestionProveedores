using System;
using System.Collections.Generic;
using Proveedores.Datos;
using Proveedores.Modelo;

namespace Proveedores.Negocio;

public class AsignacionServicio
{
    private const decimal PrecioMaximo = 9_999_999_999.99m;   // NUMBER(12,2)

    private readonly DetalleProductoDAO _detalleDao = new();
    private readonly ProveedorDAO _proveedorDao = new();

    public List<DetalleProducto> ListarDeProveedor(string cedula)
    {
        Sesion.Exigir(Permisos.Leer);
        return _detalleDao.ListarDeProveedor(cedula);
    }

    public void Guardar(string cedula, List<DetalleProducto> asignaciones)
    {
        Sesion.Exigir(Permisos.Crear);
        Sesion.Exigir(Permisos.Actualizar);
        Sesion.Exigir(Permisos.Eliminar);

        if (string.IsNullOrWhiteSpace(cedula))
            throw new ReglaNegocioException("Debe seleccionar un proveedor.");

        if (_proveedorDao.ObtenerPorCedula(cedula) is null)
            throw new ReglaNegocioException("El proveedor no existe.");

        var validas = new List<DetalleProducto>();

        // Un producto no puede repetirse: se toma la primera fila de cada uno.
        foreach (var grupo in asignaciones.GroupBy(a => a.ProductoId))
        {
            var asignacion = grupo.First();
            decimal precio = Math.Round(asignacion.Precio, 2, MidpointRounding.AwayFromZero);

            if (precio <= 0)
                throw new ReglaNegocioException(
                    "Todos los productos asignados deben tener un precio mayor que 0.");
            if (precio > PrecioMaximo)
                throw new ReglaNegocioException(
                    "El precio no puede superar 9.999.999.999,99.");

            validas.Add(new DetalleProducto
            {
                ProductoId = asignacion.ProductoId,
                ProveedorCedula = cedula,
                Precio = precio
            });
        }

        _detalleDao.ReemplazarProductosDeProveedor(cedula, validas);
    }
}