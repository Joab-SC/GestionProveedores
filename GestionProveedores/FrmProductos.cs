using Proveedores.Modelo;
using Proveedores.Negocio;

namespace GestionProveedores
{
    public partial class FrmProductos : Form
    {
        private readonly ProductoServicio servicio = new ProductoServicio();
        private readonly CategoriaServicio categoriaServicio = new CategoriaServicio();
        private int idSeleccionado = 0;   // 0 = registro nuevo

        public FrmProductos()
        {
            InitializeComponent();

            dgvProductos.ReadOnly = true;
            dgvProductos.AllowUserToAddRows = false;
            dgvProductos.AllowUserToDeleteRows = false;
            dgvProductos.MultiSelect = false;
            dgvProductos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProductos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvProductos.SelectionChanged += dgvProductos_SelectionChanged;
            btnNuevo.Click += btnNuevo_Click;
            btnGuardar.Click += btnGuardar_Click;
            btnEliminar.Click += btnEliminar_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            AplicarPermisos();
            CargarCategorias();
            Refrescar();
        }

        private void AplicarPermisos()
        {
            bool puedeCrear = Sesion.TienePermiso(Permisos.Crear);
            bool puedeActualizar = Sesion.TienePermiso(Permisos.Actualizar);

            btnNuevo.Enabled = puedeCrear;
            btnGuardar.Enabled = puedeCrear || puedeActualizar;
            btnEliminar.Enabled = Sesion.TienePermiso(Permisos.Eliminar);
        }

        private void CargarCategorias()
        {
            try
            {
                cmbCategoria.DataSource = categoriaServicio.Listar();
                cmbCategoria.DisplayMember = "Nombre";
                cmbCategoria.ValueMember = "Id";
                cmbCategoria.SelectedIndex = -1;
            }
            catch (ReglaNegocioException ex)
            {
                Avisar(ex.Message);
            }
            catch (Exception)
            {
                Avisar("No se pudo cargar las categorías. Revisa la conexión.");
            }
        }

        private void Refrescar()
        {
            try
            {
                dgvProductos.DataSource = servicio.ListarVista();

                if (dgvProductos.Columns["Id"] != null)
                    dgvProductos.Columns["Id"].Visible = false;
                if (dgvProductos.Columns["CategoriaId"] != null)
                    dgvProductos.Columns["CategoriaId"].Visible = false;
                if (dgvProductos.Columns["Descripcion"] != null)
                    dgvProductos.Columns["Descripcion"].HeaderText = "Descripción";
                if (dgvProductos.Columns["CategoriaNombre"] != null)
                    dgvProductos.Columns["CategoriaNombre"].HeaderText = "Categoría";

                Limpiar();
            }
            catch (ReglaNegocioException ex)
            {
                Avisar(ex.Message);
            }
            catch (Exception)
            {
                Avisar("No se pudo completar la operación. Revisa la conexión.");
            }
        }

        private void Limpiar()
        {
            idSeleccionado = 0;
            txtNombre.Clear();
            txtDescripcion.Clear();
            cmbCategoria.SelectedIndex = -1;
            dgvProductos.ClearSelection();
            txtNombre.Focus();
        }

        private void dgvProductos_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvProductos.SelectedRows.Count == 0) return;

            if (dgvProductos.SelectedRows[0].DataBoundItem is ProductoVistaDto producto)
            {
                idSeleccionado = producto.Id;
                txtNombre.Text = producto.Nombre;
                txtDescripcion.Text = producto.Descripcion;
                cmbCategoria.SelectedValue = producto.CategoriaId;
            }
        }

        private void btnNuevo_Click(object? sender, EventArgs e)
        {
            Limpiar();
        }

        private void btnGuardar_Click(object? sender, EventArgs e)
        {
            try
            {
                if (cmbCategoria.SelectedValue == null)
                {
                    Avisar("La categoría es obligatoria.");
                    return;
                }

                int categoriaId = Convert.ToInt32(cmbCategoria.SelectedValue);

                if (idSeleccionado == 0)
                    servicio.Crear(txtNombre.Text, txtDescripcion.Text, categoriaId);
                else
                    servicio.Editar(idSeleccionado, txtNombre.Text, txtDescripcion.Text, categoriaId);

                Refrescar();
            }
            catch (ReglaNegocioException ex)
            {
                Avisar(ex.Message);
            }
            catch (Exception)
            {
                Avisar("No se pudo completar la operación. Revisa la conexión.");
            }
        }

        private void btnEliminar_Click(object? sender, EventArgs e)
        {
            if (idSeleccionado == 0)
            {
                Avisar("Selecciona primero el producto que quieres eliminar.");
                return;
            }

            var respuesta = MessageBox.Show(
                "¿Seguro que quieres eliminar este producto? También se borrarán las asignaciones a proveedores.",
                "Confirmar",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (respuesta != DialogResult.Yes) return;

            try
            {
                servicio.Eliminar(idSeleccionado);
                Refrescar();
            }
            catch (ReglaNegocioException ex)
            {
                Avisar(ex.Message);
            }
            catch (Exception)
            {
                Avisar("No se pudo completar la operación. Revisa la conexión.");
            }
        }

        private static void Avisar(string mensaje)
        {
            MessageBox.Show(mensaje, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void btnGuardar_Click_1(object sender, EventArgs e)
        {

        }
    }
}