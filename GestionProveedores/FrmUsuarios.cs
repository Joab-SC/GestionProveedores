using Proveedores.Modelo;
using Proveedores.Negocio;

namespace GestionProveedores;

public partial class FrmUsuarios : Form
{
    private readonly UsuarioServicio usuarioServicio = new();
    private readonly CiudadServicio ciudadServicio = new();
    private readonly TipoUsuarioServicio tipoServicio = new();

    private string? cedulaSeleccionada;
    private const string Mascara = "********";

    public FrmUsuarios()
    {
        InitializeComponent();
        Load += FrmUsuarios_Load;
        dgvUsuarios.SelectionChanged += dgvUsuarios_SelectionChanged;
        btnNuevo.Click += (_, _) => Limpiar();
        btnGuardar.Click += btnGuardar_Click;
        btnDesactivar.Click += btnDesactivar_Click;

        // Manejo de la máscara de contraseña
        txtContrasena.Enter += (_, _) =>
        {
            if (txtContrasena.Text == Mascara) txtContrasena.Clear();
        };
        txtContrasena.Leave += (_, _) =>
        {
            if (cedulaSeleccionada != null && txtContrasena.Text.Length == 0)
                txtContrasena.Text = Mascara;
        };
    }

    private void FrmUsuarios_Load(object? sender, EventArgs e)
    {
        try
        {
            cmbCiudad.DisplayMember = "Nombre";
            cmbCiudad.ValueMember = "Id";
            cmbCiudad.DataSource = ciudadServicio.Listar();

            cmbTipoUsuario.DisplayMember = "Nombre";
            cmbTipoUsuario.ValueMember = "Id";
            cmbTipoUsuario.DataSource = tipoServicio.Listar();

            Refrescar();
            Limpiar();
        }
        catch (ReglaNegocioException ex) { Aviso(ex.Message); }
    }

    private void Refrescar() => dgvUsuarios.DataSource = usuarioServicio.ListarVista();

    private void Limpiar()
    {
        cedulaSeleccionada = null;
        txtCedula.ReadOnly = false;
        txtCedula.Clear();
        txtNombre.Clear();
        txtApellido.Clear();
        txtContrasena.Clear();
        dtpFechaNacimiento.Value = DateTime.Today.AddYears(-18);
        cmbCiudad.SelectedIndex = -1;
        cmbTipoUsuario.SelectedIndex = -1;
        chkActivo.Checked = true;
        dgvUsuarios.ClearSelection();
        dgvUsuarios.CurrentCell = null;
    }

    private void dgvUsuarios_SelectionChanged(object? sender, EventArgs e)
    {
        if (dgvUsuarios.CurrentRow?.DataBoundItem is not UsuarioVistaDto dto) return;

        try
        {
            var u = usuarioServicio.ObtenerPorCedula(dto.Cedula);
            if (u == null) return;

            cedulaSeleccionada = u.Cedula;
            txtCedula.Text = u.Cedula;
            txtCedula.ReadOnly = true;
            txtNombre.Text = u.Nombre;
            txtApellido.Text = u.Apellido;
            dtpFechaNacimiento.Value = u.FechaNacimiento;
            cmbCiudad.SelectedValue = u.CiudadId;
            cmbTipoUsuario.SelectedValue = u.TipoUsuarioId;
            chkActivo.Checked = u.Activo;
            txtContrasena.Text = Mascara;
        }
        catch (ReglaNegocioException ex) { Aviso(ex.Message); }
    }

    private void btnGuardar_Click(object? sender, EventArgs e)
    {
        try
        {
            var u = new Usuario
            {
                // En edición la cédula es la llave y no se puede cambiar.
                // Usamos el valor capturado al seleccionar la fila para que el
                // formulario no envíe una caja de texto vacía por accidente.
                Cedula = cedulaSeleccionada ?? txtCedula.Text,
                Nombre = txtNombre.Text,
                Apellido = txtApellido.Text,
                FechaNacimiento = dtpFechaNacimiento.Value.Date,
                CiudadId = cmbCiudad.SelectedValue is int c ? c : 0,
                TipoUsuarioId = cmbTipoUsuario.SelectedValue is int t ? t : 0,
                Activo = chkActivo.Checked
            };

            if (cedulaSeleccionada == null)
            {
                usuarioServicio.Crear(u, txtContrasena.Text);
            }
            else
            {
                // Si el campo sigue con la máscara, significa "no cambiar la contraseña"
                string? nueva = txtContrasena.Text == Mascara ? null : txtContrasena.Text;
                usuarioServicio.Editar(u, nueva);
            }

            Refrescar();
            Limpiar();
        }
        catch (ReglaNegocioException ex) { Aviso(ex.Message); }
    }

    private void btnDesactivar_Click(object? sender, EventArgs e)
    {
        if (cedulaSeleccionada == null)
        {
            Aviso("Selecciona un usuario de la lista.");
            return;
        }

        var r = MessageBox.Show("¿Desactivar este usuario? Ya no podrá iniciar sesión.",
            "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (r != DialogResult.Yes) return;

        try
        {
            usuarioServicio.Desactivar(cedulaSeleccionada);
            Refrescar();
            Limpiar();
        }
        catch (ReglaNegocioException ex) { Aviso(ex.Message); }
    }

    private static void Aviso(string mensaje) =>
        MessageBox.Show(mensaje, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
}