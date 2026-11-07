using System.ComponentModel;
using Proveedores.Modelo;
using Proveedores.Negocio;

namespace GestionProveedores;

/// <summary>
/// Permite preparar en pantalla los productos que ofrece un proveedor, con el
/// precio de cada uno, y confirmarlos en una única operación al pulsar Guardar cambios.
/// </summary>
public partial class FrmAsignaciones : Form
{
    // Fila de la tabla de asignados: el producto y el precio que ofrece este proveedor.
    private sealed class ItemAsignado
    {
        public int ProductoId { get; set; }
        public string Producto { get; set; } = string.Empty;
        public decimal Precio { get; set; }
    }

    private readonly ProveedorServicio _proveedorServicio = new();
    private readonly ProductoServicio _productoServicio = new();
    private readonly AsignacionServicio _asignacionServicio = new();

    private readonly BindingList<ItemAsignado> _asignados = new();
    private List<Producto> _todosLosProductos = new();
    private bool _cargando;

    public FrmAsignaciones()
    {
        InitializeComponent();

        // Configuración de la tabla de asignados (el precio se edita directamente en ella)
        dgvAsignados.AllowUserToAddRows = false;
        dgvAsignados.AllowUserToDeleteRows = false;
        dgvAsignados.MultiSelect = false;
        dgvAsignados.RowHeadersVisible = false;
        dgvAsignados.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvAsignados.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        Load += FrmAsignaciones_Load;
        cmbProveedor.SelectedIndexChanged += cmbProveedor_SelectedIndexChanged;
        dgvAsignados.DataError += dgvAsignados_DataError;
        btnAgregar.Click += btnAgregar_Click;
        btnQuitar.Click += btnQuitar_Click;
        btnGuardarCambios.Click += btnGuardarCambios_Click;
    }

    private void FrmAsignaciones_Load(object? sender, EventArgs e)
    {
        try
        {
            _todosLosProductos = _productoServicio.Listar();

            _cargando = true;
            cmbProveedor.DisplayMember = nameof(Proveedor.NombreCompleto);
            cmbProveedor.DataSource = _proveedorServicio.Listar();
            cmbProveedor.SelectedIndex = -1;

            lstDisponibles.DisplayMember = nameof(Producto.Nombre);

            dgvAsignados.DataSource = _asignados;
            ConfigurarColumnas();

            HabilitarEdicion(false);
            btnGuardarCambios.Enabled = Sesion.TienePermiso(Permisos.Crear)
                                     && Sesion.TienePermiso(Permisos.Actualizar)
                                     && Sesion.TienePermiso(Permisos.Eliminar);
        }
        catch (ReglaNegocioException ex)
        {
            Avisar(ex.Message);
        }
        catch (Exception)
        {
            Avisar("No se pudieron cargar los datos. Revisa la conexión.");
        }
        finally
        {
            _cargando = false;
        }
    }

    private void ConfigurarColumnas()
    {
        dgvAsignados.Columns[nameof(ItemAsignado.ProductoId)]!.Visible = false;

        var colProducto = dgvAsignados.Columns[nameof(ItemAsignado.Producto)]!;
        colProducto.HeaderText = "Producto";
        colProducto.ReadOnly = true;            // solo el precio se puede editar

        var colPrecio = dgvAsignados.Columns[nameof(ItemAsignado.Precio)]!;
        colPrecio.HeaderText = "Precio";
        colPrecio.DefaultCellStyle.Format = "N2";
        colPrecio.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
    }

    private void cmbProveedor_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_cargando || cmbProveedor.SelectedItem is not Proveedor proveedor) return;

        try
        {
            var detalles = _asignacionServicio.ListarDeProveedor(proveedor.Cedula);
            var idsAsignados = detalles.Select(d => d.ProductoId).ToHashSet();

            _asignados.Clear();
            foreach (var detalle in detalles)
            {
                var producto = _todosLosProductos.FirstOrDefault(p => p.Id == detalle.ProductoId);
                if (producto is null) continue;

                _asignados.Add(new ItemAsignado
                {
                    ProductoId = producto.Id,
                    Producto = producto.Nombre,
                    Precio = detalle.Precio
                });
            }

            var disponibles = _todosLosProductos.Where(p => !idsAsignados.Contains(p.Id)).ToList();
            lstDisponibles.Items.Clear();
            lstDisponibles.Items.AddRange(disponibles.Cast<object>().ToArray());

            HabilitarEdicion(true);
        }
        catch (ReglaNegocioException ex)
        {
            Avisar(ex.Message);
        }
        catch (Exception)
        {
            Avisar("No se pudieron cargar las asignaciones. Revisa la conexión.");
        }
    }

    // Pasa el producto elegido a la tabla de asignados, con precio 0 hasta que se escriba el real.
    private void btnAgregar_Click(object? sender, EventArgs e)
    {
        if (lstDisponibles.SelectedItem is not Producto producto) return;

        lstDisponibles.Items.Remove(producto);
        _asignados.Add(new ItemAsignado
        {
            ProductoId = producto.Id,
            Producto = producto.Nombre,
            Precio = 0m
        });

        // Deja el cursor en la celda del precio para escribirlo enseguida.
        int ultima = dgvAsignados.Rows.Count - 1;
        dgvAsignados.CurrentCell = dgvAsignados.Rows[ultima].Cells[nameof(ItemAsignado.Precio)];
        dgvAsignados.Focus();
    }

    // Devuelve el producto de la fila seleccionada a la lista de disponibles.
    private void btnQuitar_Click(object? sender, EventArgs e)
    {
        if (dgvAsignados.CurrentRow?.DataBoundItem is not ItemAsignado item) return;

        dgvAsignados.CancelEdit();
        _asignados.Remove(item);

        var producto = _todosLosProductos.FirstOrDefault(p => p.Id == item.ProductoId);
        if (producto is not null)
            lstDisponibles.Items.Add(producto);
    }

    private void btnGuardarCambios_Click(object? sender, EventArgs e)
    {
        if (cmbProveedor.SelectedItem is not Proveedor proveedor)
        {
            Avisar("Seleccione un proveedor.");
            return;
        }

        try
        {
            dgvAsignados.EndEdit();   // confirma el precio que se esté escribiendo

            var asignaciones = _asignados
                .Select(i => new DetalleProducto
                {
                    ProductoId = i.ProductoId,
                    ProveedorCedula = proveedor.Cedula,
                    Precio = i.Precio
                })
                .ToList();

            _asignacionServicio.Guardar(proveedor.Cedula, asignaciones);

            MessageBox.Show("Asignaciones guardadas correctamente.", "Listo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (ReglaNegocioException ex)
        {
            Avisar(ex.Message);
        }
        catch (Exception)
        {
            Avisar("No se pudieron guardar las asignaciones. Revisa la conexión.");
        }
    }

    // Se dispara cuando se escribe en el precio algo que no es un número.
    private void dgvAsignados_DataError(object? sender, DataGridViewDataErrorEventArgs e)
    {
        Avisar("El precio debe ser un número válido (por ejemplo, 2500 o 2500,50).");
        e.ThrowException = false;
    }

    private void HabilitarEdicion(bool habilitar)
    {
        lstDisponibles.Enabled = dgvAsignados.Enabled = habilitar;
        btnAgregar.Enabled = btnQuitar.Enabled = habilitar;
    }

    private static void Avisar(string mensaje) =>
        MessageBox.Show(mensaje, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
}