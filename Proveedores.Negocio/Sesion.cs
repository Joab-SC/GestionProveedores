using System;
using System.Collections.Generic;
using Proveedores.Modelo;

namespace Proveedores.Negocio
{
    // Sesion estatica de la aplicacion: el unico lugar donde viven el
    // usuario logueado, sus permisos y el id de la fila de bitacora.
    // Iniciar() lo llama AutenticacionServicio (CU-01); Cerrar(), el CU-02.
    // Ninguna capa guarda aqui la contrasena ni el hash.
    public static class Sesion
    {
        private static readonly HashSet<string> permisosGuardados =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public static Usuario? UsuarioActual { get; private set; }
        public static int BitacoraId { get; private set; }

        public static void Iniciar(Usuario usuario, IEnumerable<string> permisos, int bitacoraId)
        {
            // Regla 5: esta capa solo lanza ReglaNegocioException (o sus
            // derivadas); el parametro no-anulable ya protege aqui.
            UsuarioActual = usuario;
            BitacoraId = bitacoraId;

            permisosGuardados.Clear();
            if (permisos != null)
            {
                foreach (string permiso in permisos)
                {
                    if (!string.IsNullOrWhiteSpace(permiso))
                        permisosGuardados.Add(permiso.Trim().ToUpperInvariant());
                }
            }
        }

        public static void Cerrar()
        {
            UsuarioActual = null;
            BitacoraId = 0;
            permisosGuardados.Clear();
        }

        public static bool TienePermiso(string permiso)
        {
            if (UsuarioActual == null) return false;
            if (string.IsNullOrWhiteSpace(permiso)) return false;

            return permisosGuardados.Contains(permiso.Trim().ToUpperInvariant());
        }

        // Cada metodo que escribe debe empezar por Exigir(...)
        public static void Exigir(string permiso)
        {
            if (!TienePermiso(permiso))
                throw new PermisoDenegadoException(
                    "No tiene permiso para realizar esta operación.");
        }
    }
}
