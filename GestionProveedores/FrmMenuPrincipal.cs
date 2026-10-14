using Proveedores.Modelo;
using Proveedores.Negocio;

namespace GestionProveedores
{
    // Menu provisional: solo para mostrar que la sesion quedo abierta.
    public partial class FrmMenuPrincipal : Form
    {
        private readonly AutenticacionServicio _autenticacion = new();
        public FrmMenuPrincipal()
        {
            InitializeComponent();
        }

        private void FrmMenuPrincipal_Load(object sender, EventArgs e)
        {
            // CU-03: reemplazar por el menú real
            lblBienvenida.Text =
                $"Bienvenido, {Sesion.UsuarioActual!.Nombre} {Sesion.UsuarioActual.Apellido}";
        }


        // Se dispara con la X y también cuando se llama a Close().
        private void FrmMenuPrincipal_FormClosing(object? sender, FormClosingEventArgs e)
        {
            RegistrarSalida();
        }

        // Opción del menú: registra la salida y cierra el formulario.
        private void mnuCerrarSesion_Click(object sender, EventArgs e)
        {
            RegistrarSalida();
            Close();
        }

        // CU-02: cierra la sesión en la bitácora. Salir() no falla si ya se
        // llamó antes, así que la X y el botón pueden llamarlo sin duplicar nada.
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

