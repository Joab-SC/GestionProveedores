using Proveedores.Modelo;
using Proveedores.Negocio;

namespace GestionProveedores;

/// <summary>
/// Permite preparar en pantalla los productos que ofrece un proveedor y
/// confirmarlos en una única operación al pulsar Guardar cambios.
/// </summary>
public partial class FrmAsignaciones : Form
{
    private readonly ProveedorServicio _proveedorServicio = new();
    private readonly ProductoServicio _productoServicio = new();
    private readonly AsignacionServicio _asignacionServicio = new();

    private List<Producto> _todosLosProductos = new();
    private bool _cargando;

    public FrmAsignaciones()
    {
        InitializeComponent();
        Load += FrmAsignaciones_Load;
        cmbProveedor.SelectedIndexChanged += cmbProveedor_SelectedIndexChanged;
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

            // Producto no necesita cambiarse: ListBox toma su propiedad Nombre.
            lstDisponibles.DisplayMember = nameof(Producto.Nombre);
            lstAsignados.DisplayMember = nameof(Producto.Nombre);

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

    private void cmbProveedor_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_cargando || cmbProveedor.SelectedItem is not Proveedor proveedor) return;

        try
        {
            var idsAsignados = _asignacionServicio.ListarProductoIdsDeProveedor(proveedor.Cedula);
            var asignados = _todosLosProductos.Where(p => idsAsignados.Contains(p.Id)).ToList();
            var disponibles = _todosLosProductos.Where(p => !idsAsignados.Contains(p.Id)).ToList();

            lstAsignados.Items.Clear();
            lstDisponibles.Items.Clear();
            lstAsignados.Items.AddRange(asignados.Cast<object>().ToArray());
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

    private void btnAgregar_Click(object? sender, EventArgs e) => Mover(lstDisponibles, lstAsignados);

    private void btnQuitar_Click(object? sender, EventArgs e) => Mover(lstAsignados, lstDisponibles);

    private static void Mover(ListBox origen, ListBox destino)
    {
        if (origen.SelectedItem is null) return;

        object item = origen.SelectedItem;
        origen.Items.Remove(item);
        destino.Items.Add(item);
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
            var ids = lstAsignados.Items.Cast<Producto>().Select(p => p.Id).ToList();
            _asignacionServicio.Guardar(proveedor.Cedula, ids);

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

    private void HabilitarEdicion(bool habilitar)
    {
        lstDisponibles.Enabled = lstAsignados.Enabled = habilitar;
        btnAgregar.Enabled = btnQuitar.Enabled = habilitar;
    }

    private static void Avisar(string mensaje) =>
        MessageBox.Show(mensaje, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
}
