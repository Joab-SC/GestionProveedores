using Proveedores.Modelo;
using Proveedores.Negocio;

namespace GestionProveedores
{
    public partial class FrmMenuPrincipal : Form
    {
        private readonly AutenticacionServicio _autenticacion = new();

        public FrmMenuPrincipal()
        {
            InitializeComponent();

            // Eventos del formulario
            Load += FrmMenuPrincipal_Load;
            FormClosing += FrmMenuPrincipal_FormClosing;

            // Entidades: cada clic abre su formulario como ventana modal
            mnuCiudades.Click += (s, e) => Abrir(new FrmCiudades());
            mnuCategorias.Click += (s, e) => Abrir(new FrmCategorias());
            mnuProductos.Click += (s, e) => Abrir(new FrmProductos());
            mnuProveedores.Click += (s, e) => Abrir(new FrmProveedores());

            // Transacciones
            mnuAsignaciones.Click += (s, e) => Abrir(new FrmAsignaciones());

            // Módulos que aún no existen: cuando se creen, se cambia solo la línea
            // por, por ejemplo, Abrir(new FrmConsultas());
            mnuConsultas.Click += (s, e) => Pendiente("Consultas");
            mnuReportes.Click += (s, e) => Pendiente("Reportes");
            mnuUsuarios.Click += (s, e) => Pendiente("Usuarios");
            mnuBitacora.Click += (s, e) => Pendiente("Bitácora");

            // Sesión
            mnuCerrarSesion.Click += mnuCerrarSesion_Click;
        }

        private void FrmMenuPrincipal_Load(object? sender, EventArgs e)
        {
            Usuario? usuario = Sesion.UsuarioActual;
            if (usuario == null)
            {
                // Sin sesión no se debe ver el menú.
                Close();
                return;
            }

            lblBienvenida.Text = $"Bienvenido, {usuario.Nombre} {usuario.Apellido}";
            lblUsuario.Text = $"{usuario.Nombre} {usuario.Apellido}  ·  {NombreTipo(usuario.TipoUsuarioId)}";

            AplicarPermisos();
        }

        // CU-03: cada opción se muestra solo si el usuario tiene el permiso.
        // Se pregunta por permisos, nunca por el nombre del tipo de usuario.
        private void AplicarPermisos()
        {
            bool puedeGestionar =
                Sesion.TienePermiso(Permisos.Crear) ||
                Sesion.TienePermiso(Permisos.Actualizar) ||
                Sesion.TienePermiso(Permisos.Eliminar);
            bool puedeLeer = Sesion.TienePermiso(Permisos.Leer);
            bool verUsuarios = Sesion.TienePermiso(Permisos.GestionarUsuarios);
            bool verBitacora = Sesion.TienePermiso(Permisos.VerBitacora);

            mnuEntidades.Visible = puedeGestionar;
            mnuTransacciones.Visible = puedeGestionar;
            mnuConsultas.Visible = puedeLeer;
            mnuReportes.Visible = puedeLeer;
            mnuUsuarios.Visible = verUsuarios;
            mnuBitacora.Visible = verBitacora;
            mnuAuditoria.Visible = verUsuarios || verBitacora;
            mnuSesion.Visible = true;   // siempre disponible
        }

        private static string NombreTipo(int tipoUsuarioId)
        {
            return tipoUsuarioId switch
            {
                TiposUsuario.Administrador => "Administrador",
                TiposUsuario.Tradicional => "Tradicional",
                TiposUsuario.Esporadico => "Esporádico",
                _ => "Usuario"
            };
        }

        // Abre un formulario como ventana modal y lo libera al cerrarse.
        private void Abrir(Form formulario)
        {
            using (formulario)
            {
                formulario.ShowDialog(this);
            }
        }

        private static void Pendiente(string modulo)
        {
            MessageBox.Show(
                $"El módulo \"{modulo}\" aún no está disponible.",
                "Próximamente",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        // Se dispara con la X y también cuando se llama a Close().
        private void FrmMenuPrincipal_FormClosing(object? sender, FormClosingEventArgs e)
        {
            RegistrarSalida();
        }

        // Opción del menú: registra la salida y cierra el formulario.
        private void mnuCerrarSesion_Click(object? sender, EventArgs e)
        {
            RegistrarSalida();
            Close();
        }

        // CU-02: cierra la sesión en la bitácora. Salir() no falla si ya se
        // llamó antes, así que la X y el menú pueden llamarlo sin duplicar nada.
        private void RegistrarSalida()
        {
            try
            {
                _autenticacion.Salir();
            }
            catch (ReglaNegocioException)
            {
                // No bloqueamos el cierre si falla el registro de salida.
            }
        }
    }
}