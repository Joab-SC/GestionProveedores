using System;
using System.Data;
using Proveedores.Datos;
using Proveedores.Modelo;

namespace Proveedores.Negocio
{
    // Validación de filtros y permisos de los reportes. No genera el PDF: eso es de Presentación.
    public class ReporteServicio
    {
        private readonly ReporteDAO dao = new ReporteDAO();
        private readonly ProveedorDAO proveedorDao = new ProveedorDAO();

        public DataTable ProveedoresPorCiudad(int? ciudadId)
        {
            Sesion.Exigir(Permisos.Leer);
            if (ciudadId.HasValue && ciudadId.Value <= 0)
                throw new ReglaNegocioException("La ciudad elegida no es válida.");

            return dao.ProveedoresPorCiudad(ciudadId);
        }

        public DataTable ProductosDeProveedor(string cedula)
        {
            Sesion.Exigir(Permisos.Leer);

            cedula = (cedula ?? string.Empty).Trim();
            if (cedula.Length == 0)
                throw new ReglaNegocioException("Debes elegir un proveedor.");
            if (proveedorDao.ObtenerPorCedula(cedula) == null)
                throw new ReglaNegocioException("El proveedor elegido no existe.");

            return dao.ProductosDeProveedor(cedula);
        }

        public DataTable ProveedoresDeProducto(int productoId)
        {
            Sesion.Exigir(Permisos.Leer);
            if (productoId <= 0)
                throw new ReglaNegocioException("Debes elegir un producto.");

            return dao.ProveedoresDeProducto(productoId);
        }

        public DataTable ProductosPorCategoria(int? categoriaId)
        {
            Sesion.Exigir(Permisos.Leer);
            if (categoriaId.HasValue && categoriaId.Value <= 0)
                throw new ReglaNegocioException("La categoría elegida no es válida.");

            return dao.ProductosPorCategoria(categoriaId);
        }

        public DataTable ProductosBuscados(string texto, int? categoriaId)
        {
            Sesion.Exigir(Permisos.Leer);

            texto = (texto ?? string.Empty).Trim();
            if (texto.Length > 60)
                throw new ReglaNegocioException("El texto de búsqueda no puede superar 60 caracteres.");
            if (categoriaId.HasValue && categoriaId.Value <= 0)
                throw new ReglaNegocioException("La categoría elegida no es válida.");

            return dao.ProductosBuscados(texto, categoriaId);
        }
    }
}