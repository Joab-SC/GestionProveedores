using Proveedores.Modelo;
using Proveedores.Negocio;

namespace GestionProveedores
{
    public partial class FrmCiudades : Form
    {
        private readonly CiudadServicio servicio = new CiudadServicio();
        private int idSeleccionado = 0;   // 0 = registro nuevo

        public FrmCiudades()
        {
            InitializeComponent();

            // Configuración de la tabla
            dgvCiudades.ReadOnly = true;
            dgvCiudades.AllowUserToAddRows = false;
            dgvCiudades.AllowUserToDeleteRows = false;
            dgvCiudades.MultiSelect = false;
            dgvCiudades.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCiudades.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Eventos enlazados desde el código (no desde el diseñador)
            dgvCiudades.SelectionChanged += dgvCiudades_SelectionChanged;
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
                dgvCiudades.DataSource = servicio.Listar();

                if (dgvCiudades.Columns["Id"] != null)
                    dgvCiudades.Columns["Id"].Visible = false;

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
            dgvCiudades.ClearSelection();
            txtNombre.Focus();
        }

        private void dgvCiudades_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvCiudades.SelectedRows.Count == 0) return;

            if (dgvCiudades.SelectedRows[0].DataBoundItem is Ciudad ciudad)
            {
                idSeleccionado = ciudad.Id;
                txtNombre.Text = ciudad.Nombre;
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
                    servicio.Crear(txtNombre.Text);
                else
                    servicio.Editar(idSeleccionado, txtNombre.Text);

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
                Avisar("Selecciona primero la ciudad que quieres eliminar.");
                return;
            }

            var respuesta = MessageBox.Show(
                "¿Seguro que quieres eliminar esta ciudad?",
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