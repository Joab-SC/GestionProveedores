using System;
using System.Collections.Generic;
using System.Linq;
using Proveedores.Datos;
using Proveedores.Modelo;

namespace Proveedores.Negocio
{
    // Reglas de negocio de Proveedor. Los formularios hablan con esta clase, no con el DAO.
    public class ProveedorServicio
    {
        private const int EdadMinima = 18;

        private readonly ProveedorDAO dao = new ProveedorDAO();

        public List<ProveedorVistaDto> ListarVista()
        {
            Sesion.Exigir(Permisos.Leer);
            return dao.ListarVista();
        }

        public List<Proveedor> Listar()
        {
            Sesion.Exigir(Permisos.Leer);
            return dao.Listar();
        }

        public Proveedor? ObtenerPorCedula(string cedula)
        {
            Sesion.Exigir(Permisos.Leer);
            return dao.ObtenerPorCedula((cedula ?? string.Empty).Trim());
        }

        public void Crear(Proveedor proveedor)
        {
            Sesion.Exigir(Permisos.Crear);
            Normalizar(proveedor);
            Validar(proveedor);
            try
            {
                dao.Insertar(proveedor);
            }
            catch (DatoDuplicadoException)
            {
                throw new DatoDuplicadoException("Ya existe un proveedor con esa cédula.");
            }
        }

        public void Editar(Proveedor proveedor)
        {
            Sesion.Exigir(Permisos.Actualizar);
            Normalizar(proveedor);
            Validar(proveedor);

            int filas = dao.Actualizar(proveedor);
            if (filas == 0)
                throw new ReglaNegocioException("El proveedor ya no existe.");
        }

        public void Eliminar(string cedula)
        {
            Sesion.Exigir(Permisos.Eliminar);

            // Sus asignaciones de productos se borran solas (ON DELETE CASCADE).
            int filas = dao.Eliminar((cedula ?? string.Empty).Trim());
            if (filas == 0)
                throw new ReglaNegocioException("El proveedor ya no existe.");
        }

        private static void Normalizar(Proveedor proveedor)
        {
            proveedor.Cedula = (proveedor.Cedula ?? string.Empty).Trim();
            proveedor.Nombre = (proveedor.Nombre ?? string.Empty).Trim();
            proveedor.Apellido = (proveedor.Apellido ?? string.Empty).Trim();
            proveedor.FechaNacimiento = proveedor.FechaNacimiento.Date;
        }

        private static void Validar(Proveedor proveedor)
        {
            if (proveedor.Cedula.Length == 0)
                throw new ReglaNegocioException("La cédula es obligatoria.");
            if (!proveedor.Cedula.All(c => c >= '0' && c <= '9'))
                throw new ReglaNegocioException("La cédula solo puede tener números.");
            if (proveedor.Cedula.Length < 6 || proveedor.Cedula.Length > 15)
                throw new ReglaNegocioException("La cédula debe tener entre 6 y 15 dígitos.");

            if (proveedor.Nombre.Length == 0)
                throw new ReglaNegocioException("El nombre es obligatorio.");
            if (proveedor.Nombre.Length > 60)
                throw new ReglaNegocioException("El nombre no puede superar 60 caracteres.");

            if (proveedor.Apellido.Length == 0)
                throw new ReglaNegocioException("El apellido es obligatorio.");
            if (proveedor.Apellido.Length > 60)
                throw new ReglaNegocioException("El apellido no puede superar 60 caracteres.");

            DateTime hoy = DateTime.Today;
            if (proveedor.FechaNacimiento > hoy)
                throw new ReglaNegocioException("La fecha de nacimiento no puede ser futura.");

            int edad = hoy.Year - proveedor.FechaNacimiento.Year;
            if (proveedor.FechaNacimiento > hoy.AddYears(-edad)) edad--;
            if (edad < EdadMinima)
                throw new ReglaNegocioException($"El proveedor debe tener al menos {EdadMinima} años.");

            if (proveedor.CiudadId <= 0)
                throw new ReglaNegocioException("Debes elegir una ciudad.");
        }
    }
}