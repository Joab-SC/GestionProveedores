using Proveedores.Modelo;
using Proveedores.Negocio;

namespace GestionProveedores
{
    public partial class FrmProveedores : Form
    {
        private readonly ProveedorServicio servicio = new ProveedorServicio();
        private readonly CiudadServicio ciudadServicio = new CiudadServicio();
        private bool esNuevo = true;
        private bool cargando = false;

        public FrmProveedores()
        {
            InitializeComponent();

            // Configuración de la tabla
            dgvProveedores.ReadOnly = true;
            dgvProveedores.AllowUserToAddRows = false;
            dgvProveedores.AllowUserToDeleteRows = false;
            dgvProveedores.MultiSelect = false;
            dgvProveedores.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProveedores.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Límites de la fecha de nacimiento
            dtpFechaNacimiento.MinDate = new DateTime(1900, 1, 1);
            dtpFechaNacimiento.MaxDate = DateTime.Today;

            // Eventos enlazados desde el código (no desde el diseñador)
            dgvProveedores.SelectionChanged += dgvProveedores_SelectionChanged;
            btnNuevo.Click += btnNuevo_Click;
            btnGuardar.Click += btnGuardar_Click;
            btnEliminar.Click += btnEliminar_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            AplicarPermisos();
            CargarCiudades();
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

        private void CargarCiudades()
        {
            try
            {
                cmbCiudad.DisplayMember = "Nombre";
                cmbCiudad.ValueMember = "Id";
                cmbCiudad.DataSource = ciudadServicio.Listar();
                cmbCiudad.SelectedIndex = -1;
            }
            catch (ReglaNegocioException ex)
            {
                Avisar(ex.Message);
            }
            catch (Exception)
            {
                Avisar("No se pudieron cargar las ciudades. Revisa la conexión.");
            }
        }

        private void Refrescar()
        {
            try
            {
                cargando = true;
                dgvProveedores.DataSource = servicio.ListarVista();

                if (dgvProveedores.Columns["Cedula"] != null)
                    dgvProveedores.Columns["Cedula"].HeaderText = "Cédula";
                if (dgvProveedores.Columns["NombreCompleto"] != null)
                    dgvProveedores.Columns["NombreCompleto"].HeaderText = "Nombre";
            }
            catch (ReglaNegocioException ex)
            {
                Avisar(ex.Message);
            }
            catch (Exception)
            {
                Avisar("No se pudo completar la operación. Revisa la conexión.");
            }
            finally
            {
                cargando = false;
            }

            Limpiar();
        }

        private void Limpiar()
        {
            esNuevo = true;
            txtCedula.ReadOnly = false;
            txtCedula.Clear();
            txtNombre.Clear();
            txtApellido.Clear();
            dtpFechaNacimiento.Value = DateTime.Today.AddYears(-18);
            cmbCiudad.SelectedIndex = -1;
            dgvProveedores.ClearSelection();
            txtCedula.Focus();
        }

        private void dgvProveedores_SelectionChanged(object? sender, EventArgs e)
        {
            if (cargando || dgvProveedores.SelectedRows.Count == 0) return;
            if (dgvProveedores.SelectedRows[0].DataBoundItem is not ProveedorVistaDto fila) return;

            try
            {
                Proveedor? proveedor = servicio.ObtenerPorCedula(fila.Cedula);
                if (proveedor == null)
                {
                    Avisar("El proveedor ya no existe.");
                    Refrescar();
                    return;
                }

                esNuevo = false;
                txtCedula.Text = proveedor.Cedula;
                txtCedula.ReadOnly = true;      // la cédula es la llave: no se edita
                txtNombre.Text = proveedor.Nombre;
                txtApellido.Text = proveedor.Apellido;

                DateTime fecha = proveedor.FechaNacimiento;
                if (fecha < dtpFechaNacimiento.MinDate) fecha = dtpFechaNacimiento.MinDate;
                if (fecha > dtpFechaNacimiento.MaxDate) fecha = dtpFechaNacimiento.MaxDate;
                dtpFechaNacimiento.Value = fecha;

                cmbCiudad.SelectedValue = proveedor.CiudadId;
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

        private void btnNuevo_Click(object? sender, EventArgs e)
        {
            Limpiar();
        }

        private void btnGuardar_Click(object? sender, EventArgs e)
        {
            try
            {
                var proveedor = new Proveedor
                {
                    Cedula = txtCedula.Text,
                    Nombre = txtNombre.Text,
                    Apellido = txtApellido.Text,
                    FechaNacimiento = dtpFechaNacimiento.Value.Date,
                    CiudadId = cmbCiudad.SelectedValue is int id ? id : 0
                };

                if (esNuevo)
                    servicio.Crear(proveedor);
                else
                    servicio.Editar(proveedor);

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
            if (esNuevo)
            {
                Avisar("Selecciona primero el proveedor que quieres eliminar.");
                return;
            }

            var respuesta = MessageBox.Show(
                "¿Seguro que quieres eliminar este proveedor?\n" +
                "También se borrarán los productos que tiene asignados.",
                "Confirmar",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (respuesta != DialogResult.Yes) return;

            try
            {
                servicio.Eliminar(txtCedula.Text);
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