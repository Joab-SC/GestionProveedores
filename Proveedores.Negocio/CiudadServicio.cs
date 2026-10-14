using System;
using System.Collections.Generic;
using Proveedores.Datos;
using Proveedores.Modelo;

namespace Proveedores.Negocio
{
    // Reglas de negocio de Ciudad. Los formularios hablan con esta clase, no con el DAO.
    // Esta capa no conoce Oracle: el DAO traduce los errores a
    // ReglaNegocioException y sus derivadas antes de subirlos.
    public class CiudadServicio
    {
        private readonly CiudadDAO dao = new CiudadDAO();

        public List<Ciudad> Listar()
        {
            return dao.Listar();
        }

        public void Crear(string nombre)
        {
            Validar(nombre);
            try
            {
                dao.Insertar(new Ciudad { Nombre = nombre.Trim() });
            }
            catch (DatoDuplicadoException)
            {
                // El DAO ya tradujo el error de Oracle; aqui se pone el
                // mensaje propio de la ciudad.
                throw new DatoDuplicadoException("Ya existe una ciudad con ese nombre.");
            }
        }

        public void Editar(int id, string nombre)
        {
            Validar(nombre);
            try
            {
                dao.Actualizar(new Ciudad { Id = id, Nombre = nombre.Trim() });
            }
            catch (DatoDuplicadoException)
            {
                throw new DatoDuplicadoException("Ya existe una ciudad con ese nombre.");
            }
        }

        public void Eliminar(int id)
        {
            try
            {
                dao.Eliminar(id);
            }
            catch (RegistroEnUsoException)
            {
                throw new RegistroEnUsoException(
                    "No se puede eliminar: hay proveedores o usuarios en esta ciudad.");
            }
        }

        private static void Validar(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ReglaNegocioException("El nombre de la ciudad es obligatorio.");
            if (nombre.Trim().Length > 80)
                throw new ReglaNegocioException("El nombre no puede superar 80 caracteres.");
        }
    }
}