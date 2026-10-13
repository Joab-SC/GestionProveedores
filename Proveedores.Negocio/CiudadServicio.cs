using System;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using Proveedores.Datos;
using Proveedores.Modelo;

namespace Proveedores.Negocio
{
    // Reglas de negocio de Ciudad. Los formularios hablan con esta clase, no con el DAO.
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
            catch (OracleException ex) when (ex.Number == 1) // ORA-00001: nombre repetido
            {
                throw new InvalidOperationException("Ya existe una ciudad con ese nombre.");
            }
        }

        public void Editar(int id, string nombre)
        {
            Validar(nombre);
            try
            {
                dao.Actualizar(new Ciudad { Id = id, Nombre = nombre.Trim() });
            }
            catch (OracleException ex) when (ex.Number == 1)
            {
                throw new InvalidOperationException("Ya existe una ciudad con ese nombre.");
            }
        }

        public void Eliminar(int id)
        {
            try
            {
                dao.Eliminar(id);
            }
            catch (OracleException ex) when (ex.Number == 2292) // ORA-02292: tiene registros hijos
            {
                throw new InvalidOperationException(
                    "No se puede eliminar: hay proveedores o usuarios en esta ciudad.");
            }
        }

        private static void Validar(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre de la ciudad es obligatorio.");
            if (nombre.Trim().Length > 80)
                throw new ArgumentException("El nombre no puede superar 80 caracteres.");
        }
    }
}