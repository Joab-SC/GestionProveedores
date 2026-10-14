using System;
using System.Collections.Generic;
using Proveedores.Datos;
using Proveedores.Modelo;

namespace Proveedores.Negocio
{
    // Reglas de negocio de Categoría. Los formularios hablan con esta clase, no con el DAO.
    public class CategoriaServicio
    {
        private readonly CategoriaDAO dao = new CategoriaDAO();

        public List<CategoriaProducto> Listar()
        {
            Sesion.Exigir(Permisos.Leer);
            return dao.Listar();
        }

        public void Crear(string nombre, string descripcion)
        {
            Sesion.Exigir(Permisos.Crear);
            Validar(nombre, descripcion);
            try
            {
                dao.Insertar(Armar(0, nombre, descripcion));
            }
            catch (DatoDuplicadoException)
            {
                throw new DatoDuplicadoException("Ya existe una categoría con ese nombre.");
            }
        }

        public void Editar(int id, string nombre, string descripcion)
        {
            Sesion.Exigir(Permisos.Actualizar);
            Validar(nombre, descripcion);
            try
            {
                dao.Actualizar(Armar(id, nombre, descripcion));
            }
            catch (DatoDuplicadoException)
            {
                throw new DatoDuplicadoException("Ya existe una categoría con ese nombre.");
            }
        }

        public void Eliminar(int id)
        {
            Sesion.Exigir(Permisos.Eliminar);
            try
            {
                dao.Eliminar(id);
            }
            catch (RegistroEnUsoException)
            {
                throw new RegistroEnUsoException(
                    "No se puede eliminar: hay productos en esta categoría.");
            }
        }

        private static CategoriaProducto Armar(int id, string nombre, string descripcion)
        {
            return new CategoriaProducto
            {
                Id = id,
                Nombre = (nombre ?? string.Empty).Trim(),
                Descripcion = (descripcion ?? string.Empty).Trim()
            };
        }

        private static void Validar(string nombre, string descripcion)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ReglaNegocioException("El nombre de la categoría es obligatorio.");
            if (nombre.Trim().Length > 60)
                throw new ReglaNegocioException("El nombre no puede superar 60 caracteres.");
            if ((descripcion ?? string.Empty).Trim().Length > 250)
                throw new ReglaNegocioException("La descripción no puede superar 250 caracteres.");
        }
    }
}