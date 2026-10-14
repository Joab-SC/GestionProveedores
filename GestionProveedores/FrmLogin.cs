using Proveedores.Modelo;
using Proveedores.Negocio;

namespace GestionProveedores
{
    public partial class FrmLogin : Form
    {
        private const int MaxIntentos = 3;
        private int intentos;

        public FrmLogin()
        {
            InitializeComponent();
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            lblMensaje.Text = string.Empty;

            string cedula = txtCedula.Text.Trim();
            string contrasena = txtContrasena.Text;

            // Validacion en pantalla: sin consultar la base.
            if (cedula.Length == 0 || contrasena.Length == 0)
            {
                lblMensaje.Text = "Ingrese su cédula y su contraseña.";
                if (cedula.Length == 0)
                {
                    txtCedula.Focus();
                }
                else
                {
                    txtContrasena.Focus();
                }
                return;
            }

            try
            {
                new AutenticacionServicio().Ingresar(cedula, contrasena);
                DialogResult = DialogResult.OK;
            }
            catch (ReglaNegocioException ex)
            {
                lblMensaje.Text = ex.Message;
                txtContrasena.Clear();
                txtContrasena.Focus();
                intentos++;

                if (intentos >= MaxIntentos)
                {
                    MessageBox.Show(
                        "Demasiados intentos fallidos. La aplicación se cerrará.",
                        "Aviso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    DialogResult = DialogResult.Cancel;
                }
            }
            catch (Exception)
            {
                // Presentación no referencia Datos, por eso aquí no se puede
                // atrapar el error del controlador: solo se oculta el fallo tecnico.
                MessageBox.Show(
                    "No se pudo conectar con el sistema. Intente de nuevo más tarde.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
