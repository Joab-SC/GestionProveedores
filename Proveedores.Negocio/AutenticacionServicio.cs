using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Proveedores.Datos;
using Proveedores.Modelo;

namespace Proveedores.Negocio
{
    // CU-01: validar cedula + contrasena y abrir la sesion.
    // No llama Sesion.Exigir: iniciar sesion no exige permiso.
    public class AutenticacionServicio
    {
        // Unico mensaje de rechazo: no revela si fallo la cedula,
        // la contrasena o el estado del usuario.
        public const string Mensaje = "Cédula o contraseña incorrectos.";

        private readonly UsuarioDAO usuarioDao = new UsuarioDAO();
        private readonly PermisoDAO permisoDao = new PermisoDAO();
        private readonly BitacoraDAO bitacoraDao = new BitacoraDAO();

        public void Ingresar(string cedula, string contrasena)
        {
            if (string.IsNullOrWhiteSpace(cedula) || string.IsNullOrWhiteSpace(contrasena))
                throw new ReglaNegocioException(Mensaje);

            cedula = cedula.Trim();   // la contrasena no se recorta

            // Se calcula siempre, exista o no el usuario, para que el
            // rechazo tarde lo mismo en cualquier caso.
            string hash = HashContrasena.Calcular(contrasena);

            Usuario? usuario = usuarioDao.ObtenerPorCedula(cedula);

            if (usuario == null || !usuario.Activo || !Coincide(hash, usuario.Contrasena))
                throw new ReglaNegocioException(Mensaje);

            // Primero permisos y despues bitacora: si falla la carga de
            // permisos, no queda una fila de bitacora huérfana.
            List<Permiso> permisosTipo = permisoDao.ListarPorTipoUsuario(usuario.TipoUsuarioId);
            var permisos = new List<string>();
            foreach (Permiso permiso in permisosTipo)
            {
                permisos.Add(permiso.Nombre);
            }

            int bitacoraId = bitacoraDao.RegistrarEntrada(usuario.Cedula);

            // El hash no debe vivir en la sesion.
            usuario.Contrasena = string.Empty;

            Sesion.Iniciar(usuario, permisos, bitacoraId);
        }

        // Comparacion en tiempo fijo: no filtra informacion por el tiempo.
        // Si las longitudes difieren, FixedTimeEquals devuelve false.
        private static bool Coincide(string hashCalculado, string hashGuardado)
        {
            byte[] izquierda = Encoding.UTF8.GetBytes(hashCalculado);
            byte[] derecha = Encoding.UTF8.GetBytes(hashGuardado ?? string.Empty);
            return CryptographicOperations.FixedTimeEquals(izquierda, derecha);
        }
    }
}
