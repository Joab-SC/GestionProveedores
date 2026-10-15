using Proveedores.Modelo;
using Proveedores.Negocio;

namespace GestionProveedores;

public partial class FrmBitacora : Form
{
    private readonly BitacoraServicio bitacoraServicio = new();
    private readonly UsuarioServicio usuarioServicio = new();

    public FrmBitacora()
    {
        InitializeComponent();
        Load += FrmBitacora_Load;
        btnFiltrar.Click += btnFiltrar_Click;
        dgvBitacora.CellFormatting += dgvBitacora_CellFormatting;
    }

    private void FrmBitacora_Load(object? sender, EventArgs e)
    {
        try
        {
            CargarUsuarios();
            dtpDesde.Value = DateTime.Today.AddDays(-30);
            dtpHasta.Value = DateTime.Today;
            Filtrar();
        }
        catch (ReglaNegocioException ex)
        {
            MostrarAviso(ex.Message);
        }
    }

    private void CargarUsuarios()
    {
        var usuarios = usuarioServicio.ListarVista();
        usuarios.Insert(0, new UsuarioVistaDto { Cedula = "", NombreCompleto = "Todos" });

        cmbUsuario.DisplayMember = nameof(UsuarioVistaDto.NombreCompleto);
        cmbUsuario.ValueMember = nameof(UsuarioVistaDto.Cedula);
        cmbUsuario.DataSource = usuarios;
        cmbUsuario.SelectedIndex = 0;
    }

    private void btnFiltrar_Click(object? sender, EventArgs e)
    {
        try
        {
            Filtrar();
        }
        catch (ReglaNegocioException ex)
        {
            MostrarAviso(ex.Message);
        }
    }

    private void Filtrar()
    {
        if (cmbUsuario.SelectedItem is not UsuarioVistaDto seleccionado)
            return;

        dgvBitacora.DataSource = string.IsNullOrEmpty(seleccionado.Cedula)
            ? bitacoraServicio.ListarTodos(dtpDesde.Value, dtpHasta.Value)
            : bitacoraServicio.ListarPorUsuario(seleccionado.Cedula, dtpDesde.Value, dtpHasta.Value);
        ConfigurarColumnas();
    }

    private void ConfigurarColumnas()
    {
        dgvBitacora.Columns[nameof(BitacoraVistaDto.Usuario)].HeaderText = "Usuario";
        dgvBitacora.Columns[nameof(BitacoraVistaDto.FechaEntrada)].HeaderText = "Entrada";
        dgvBitacora.Columns[nameof(BitacoraVistaDto.FechaSalida)].HeaderText = "Salida";

        dgvBitacora.Columns[nameof(BitacoraVistaDto.FechaEntrada)].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
        dgvBitacora.Columns[nameof(BitacoraVistaDto.FechaSalida)].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
    }

    private void dgvBitacora_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0) return;

        var columna = dgvBitacora.Columns[e.ColumnIndex];
        if (columna.DataPropertyName == nameof(BitacoraVistaDto.FechaSalida)
            && (e.Value == null || e.Value == DBNull.Value))
        {
            e.Value = "sesión sin cierre";
            e.FormattingApplied = true;
        }
    }

    private static void MostrarAviso(string mensaje) =>
        MessageBox.Show(mensaje, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);

    private void btnFiltrar_Click_1(object sender, EventArgs e)
    {

    }
}
