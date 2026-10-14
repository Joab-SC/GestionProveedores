using Proveedores.Negocio;

namespace GestionProveedores
{
    // Menu provisional: solo para mostrar que la sesion quedo abierta.
    public partial class FrmMenuPrincipal : Form
    {
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
    }
}
