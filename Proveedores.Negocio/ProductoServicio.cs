
using System;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using Proveedores.Datos;
using Proveedores.Modelo;

namespace Proveedores.Negocio
{
    // Reglas de negocio de Producto. Los formularios hablan con esta clase, no con el DAO.
    public class ProductoServicio
    {
        private readonly ProductoDAO dao = new ProductoDAO();

        public List<ProductoVistaDto> ListarVista()
        {
            Sesion.Exigir(Permisos.Leer);
            return dao.ListarVista();
        }

        public List<Producto> Listar()
        {
            Sesion.Exigir(Permisos.Leer);
            return dao.Listar();
        }

        public void Crear(string nombre, string descripcion, int categoriaId)
        {
            Sesion.Exigir(Permisos.Crear);
            Validar(nombre, descripcion, categoriaId);
            dao.Insertar(Armar(0, nombre, descripcion, categoriaId));
        }

        public void Editar(int id, string nombre, string descripcion, int categoriaId)
        {
            Sesion.Exigir(Permisos.Actualizar);
            Validar(nombre, descripcion, categoriaId);

            int filas = dao.Actualizar(Armar(id, nombre, descripcion, categoriaId));
            if (filas == 0)
                throw new ReglaNegocioException("El producto ya no existe.");
        }

        public void Eliminar(int id)
        {
            Sesion.Exigir(Permisos.Eliminar);
            dao.Eliminar(id);
        }

        private static Producto Armar(int id, string nombre, string descripcion, int categoriaId)
        {
            return new Producto
            {
                Id = id,
                Nombre = (nombre ?? string.Empty).Trim(),
                Descripcion = (descripcion ?? string.Empty).Trim(),
                CategoriaId = categoriaId
            };
        }

        private static void Validar(string nombre, string descripcion, int categoriaId)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ReglaNegocioException("El nombre del producto es obligatorio.");
            if (nombre.Trim().Length > 60)
                throw new ReglaNegocioException("El nombre no puede superar 60 caracteres.");
            if ((descripcion ?? string.Empty).Trim().Length > 250)
                throw new ReglaNegocioException("La descripción no puede superar 250 caracteres.");
            if (categoriaId <= 0)
                throw new ReglaNegocioException("Debe seleccionar una categoría.");
        }
    }
}