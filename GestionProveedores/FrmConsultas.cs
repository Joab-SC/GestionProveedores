using Proveedores.Modelo;
using Proveedores.Negocio;

namespace GestionProveedores;

public partial class FrmConsultas : Form
{
    private readonly ConsultaServicio servicio = new();

    // Cada opción conserva el texto explicativo y la operación que la ejecuta.
    private record OpcionConsulta(string Nombre, string Descripcion, Func<object> Ejecutar)
    {
        public override string ToString() => Nombre;
    }

    public FrmConsultas()
    {
        InitializeComponent();

        Load += FrmConsultas_Load;
        cmbConsulta.SelectedIndexChanged += cmbConsulta_SelectedIndexChanged;
        btnEjecutar.Click += btnEjecutar_Click;

        dgvResultado.ReadOnly = true;
        dgvResultado.AllowUserToAddRows = false;
        dgvResultado.AllowUserToDeleteRows = false;
        dgvResultado.AllowUserToOrderColumns = true;
        dgvResultado.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvResultado.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
    }

    private void FrmConsultas_Load(object? sender, EventArgs e)
    {
        cmbConsulta.Items.AddRange(new object[]
        {
        new OpcionConsulta(
            "1. Proveedores con ciudad y productos",
            "Muestra cada proveedor con la ciudad donde está y los productos que ofrece. " +
            "Un proveedor aparece una vez por cada producto.",
            servicio.ObtenerProveedoresConCiudadYProductos),

        new OpcionConsulta(
            "2. Productos por categoría",
            "Muestra los productos organizados por categoría y cuántos proveedores ofrece cada uno. " +
            "Si nadie lo ofrece, aparece con 0.",
            servicio.ObtenerProductosPorCategoriaConProveedores),

        new OpcionConsulta(
            "3. Usuarios con ciudad, tipo y permisos",
            "Muestra los usuarios del sistema con su ciudad, su tipo de usuario y cada permiso que tienen. " +
            "Un usuario aparece una vez por cada permiso.",
            servicio.ObtenerUsuariosConCiudadTipoYPermisos),

        new OpcionConsulta(
            "4. Proveedores sobre el promedio",
            "Muestra los proveedores que ofrecen más productos que el promedio de todos los proveedores.",
            servicio.ObtenerProveedoresConMasProductosQuePromedio),

        new OpcionConsulta(
            "5. Productos sin proveedor",
            "Muestra los productos que todavía no ofrece ningún proveedor. " +
            "Si todos tienen proveedor, la lista sale vacía.",
            servicio.ObtenerProductosSinProveedor),
        });

        cmbConsulta.SelectedIndex = 0;
    }

    private void cmbConsulta_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (cmbConsulta.SelectedItem is not OpcionConsulta opcion) return;

        lblDescripcion.Text = opcion.Descripcion;
        dgvResultado.DataSource = null;
    }

    private void btnEjecutar_Click(object? sender, EventArgs e)
    {
        if (cmbConsulta.SelectedItem is not OpcionConsulta opcion) return;

        try
        {
            dgvResultado.DataSource = opcion.Ejecutar();
        }
        catch (ReglaNegocioException ex)
        {
            MessageBox.Show(ex.Message, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception)
        {
            MessageBox.Show("Ocurrió un error inesperado al ejecutar la consulta.",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
