using Proveedores.Modelo;
using Proveedores.Negocio;

namespace GestionProveedores
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // TEMPORAL: sesión de pruebas. NO subir a Git.
            Sesion.Iniciar(
                new Usuario { Cedula = "0", Nombre = "Dev", Apellido = "Temporal", TipoUsuarioId = TiposUsuario.Administrador },
                new[] { Permisos.Crear, Permisos.Leer, Permisos.Actualizar, Permisos.Eliminar },
                0);

            Application.Run(new FrmCategorias());
        }
    }
}