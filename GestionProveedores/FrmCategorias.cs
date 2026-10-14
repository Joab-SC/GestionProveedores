using Proveedores.Modelo;
using Proveedores.Negocio;

namespace GestionProveedores
{
    public partial class FrmCategorias : Form
    {
        private readonly CategoriaServicio servicio = new CategoriaServicio();
        private int idSeleccionado = 0;   // 0 = registro nuevo

        public FrmCategorias()
        {
            InitializeComponent();

            // Configuración de la tabla
            dgvCategorias.ReadOnly = true;
            dgvCategorias.AllowUserToAddRows = false;
            dgvCategorias.AllowUserToDeleteRows = false;
            dgvCategorias.MultiSelect = false;
            dgvCategorias.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCategorias.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Eventos enlazados desde el código (no desde el diseñador)
            dgvCategorias.SelectionChanged += dgvCategorias_SelectionChanged;
            btnNuevo.Click += btnNuevo_Click;
            btnGuardar.Click += btnGuardar_Click;
            btnEliminar.Click += btnEliminar_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            AplicarPermisos();
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

        private void Refrescar()
        {
            try
            {
                dgvCategorias.DataSource = servicio.Listar();

                if (dgvCategorias.Columns["Id"] != null)
                    dgvCategorias.Columns["Id"].Visible = false;
                if (dgvCategorias.Columns["Descripcion"] != null)
                    dgvCategorias.Columns["Descripcion"].HeaderText = "Descripción";

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
            dgvCategorias.ClearSelection();
            txtNombre.Focus();
        }

        private void dgvCategorias_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvCategorias.SelectedRows.Count == 0) return;

            if (dgvCategorias.SelectedRows[0].DataBoundItem is CategoriaProducto categoria)
            {
                idSeleccionado = categoria.Id;
                txtNombre.Text = categoria.Nombre;
                txtDescripcion.Text = categoria.Descripcion;
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
                if (idSeleccionado == 0)
                    servicio.Crear(txtNombre.Text, txtDescripcion.Text);
                else
                    servicio.Editar(idSeleccionado, txtNombre.Text, txtDescripcion.Text);

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
                Avisar("Selecciona primero la categoría que quieres eliminar.");
                return;
            }

            var respuesta = MessageBox.Show(
                "¿Seguro que quieres eliminar esta categoría?",
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
    }
}