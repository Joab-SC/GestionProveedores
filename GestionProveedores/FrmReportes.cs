using System.Data;
using System.Diagnostics;
using GestionProveedores.Reportes;
using Proveedores.Modelo;
using Proveedores.Negocio;

namespace GestionProveedores
{
    public partial class FrmReportes : Form
    {
        private static readonly string[] NombresReportes =
        {
            "1. Proveedores por ciudad, con sus productos",
            "2. Productos que ofrece un proveedor",
            "3. Proveedores de un producto",
            "4. Productos por categoría",
            "5. Productos con su categoría, buscados por nombre"
        };

        private static readonly string[] Titulos =
        {
            "Proveedores por ciudad",
            "Productos de un proveedor",
            "Proveedores de un producto",
            "Productos por categoría",
            "Búsqueda de productos"
        };

        private readonly ReporteServicio servicio = new ReporteServicio();
        private readonly CiudadServicio ciudadServicio = new CiudadServicio();
        private readonly ProveedorServicio proveedorServicio = new ProveedorServicio();
        private readonly ProductoServicio productoServicio = new ProductoServicio();
        private readonly CategoriaServicio categoriaServicio = new CategoriaServicio();

        // Opción de las listas de filtro. Valor = null significa "Todas".
        private sealed class OpcionFiltro
        {
            public string? Valor { get; }
            public string Texto { get; }

            public OpcionFiltro(string? valor, string texto)
            {
                Valor = valor;
                Texto = texto;
            }

            public override string ToString()
            {
                return Texto;
            }
        }

        public FrmReportes()
        {
            InitializeComponent();

            // Eventos enlazados desde el código (no desde el diseñador)
            cmbReporte.SelectedIndexChanged += cmbReporte_SelectedIndexChanged;
            btnGenerar.Click += btnGenerar_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            btnGenerar.Enabled = Sesion.TienePermiso(Permisos.Leer);

            cmbReporte.Items.AddRange(NombresReportes);
            cmbReporte.SelectedIndex = 0;   // dispara ConfigurarFiltros
        }

        private void cmbReporte_SelectedIndexChanged(object? sender, EventArgs e)
        {
            ConfigurarFiltros();
        }

        // Cambia las opciones del filtro según el reporte elegido.
        private void ConfigurarFiltros()
        {
            int reporte = cmbReporte.SelectedIndex;
            if (reporte < 0) return;

            bool pideTexto = reporte == 4;
            lblTexto.Visible = pideTexto;
            txtFiltro.Visible = pideTexto;
            txtFiltro.Clear();

            var opciones = new List<OpcionFiltro>();

            try
            {
                switch (reporte)
                {
                    case 0:
                        lblFiltro.Text = "Ciudad";
                        opciones.Add(new OpcionFiltro(null, "Todas"));
                        foreach (var ciudad in ciudadServicio.Listar())
                            opciones.Add(new OpcionFiltro(ciudad.Id.ToString(), ciudad.Nombre));
                        break;

                    case 1:
                        lblFiltro.Text = "Proveedor";
                        foreach (var proveedor in proveedorServicio.Listar())
                            opciones.Add(new OpcionFiltro(
                                proveedor.Cedula, $"{proveedor.NombreCompleto} ({proveedor.Cedula})"));
                        break;

                    case 2:
                        lblFiltro.Text = "Producto";
                        foreach (var producto in productoServicio.Listar())
                            opciones.Add(new OpcionFiltro(producto.Id.ToString(), producto.Nombre));
                        break;

                    default:   // reportes 4 y 5
                        lblFiltro.Text = "Categoría";
                        opciones.Add(new OpcionFiltro(null, "Todas"));
                        foreach (var categoria in categoriaServicio.Listar())
                            opciones.Add(new OpcionFiltro(categoria.Id.ToString(), categoria.Nombre));
                        break;
                }
            }
            catch (ReglaNegocioException ex)
            {
                Avisar(ex.Message);
            }
            catch (Exception)
            {
                Avisar("No se pudieron cargar las opciones del filtro. Revisa la conexión.");
            }

            cmbFiltro.DataSource = opciones;

            // Los reportes 2 y 3 exigen elegir; los demás empiezan en "Todas".
            bool exigeElegir = reporte == 1 || reporte == 2;
            cmbFiltro.SelectedIndex = (opciones.Count > 0 && !exigeElegir) ? 0 : -1;
        }

        private void btnGenerar_Click(object? sender, EventArgs e)
        {
            int reporte = cmbReporte.SelectedIndex;
            if (reporte < 0)
            {
                Avisar("Elige un reporte.");
                return;
            }

            var opcion = cmbFiltro.SelectedItem as OpcionFiltro;
            bool exigeElegir = reporte == 1 || reporte == 2;
            if (exigeElegir && opcion?.Valor == null)
            {
                Avisar($"Elige un valor en \"{lblFiltro.Text}\" para generar el reporte.");
                return;
            }

            try
            {
                DataTable tabla;
                string filtro;
                string textoOpcion = opcion?.Texto ?? "Todas";

                switch (reporte)
                {
                    case 0:
                        tabla = servicio.ProveedoresPorCiudad(IdDe(opcion));
                        filtro = $"Ciudad: {textoOpcion}";
                        break;
                    case 1:
                        tabla = servicio.ProductosDeProveedor(opcion!.Valor!);
                        filtro = $"Proveedor: {textoOpcion}";
                        break;
                    case 2:
                        tabla = servicio.ProveedoresDeProducto(IdDe(opcion) ?? 0);
                        filtro = $"Producto: {textoOpcion}";
                        break;
                    case 3:
                        tabla = servicio.ProductosPorCategoria(IdDe(opcion));
                        filtro = $"Categoría: {textoOpcion}";
                        break;
                    case 4:
                        tabla = servicio.ProductosBuscados(txtFiltro.Text, IdDe(opcion));
                        string texto = txtFiltro.Text.Trim();
                        filtro = $"Nombre contiene: \"{(texto.Length == 0 ? "(cualquiera)" : texto)}\"  ·  Categoría: {textoOpcion}";
                        break;
                    default:
                        return;
                }

                string? ruta = PedirRuta(Titulos[reporte]);
                if (ruta == null) return;   // el usuario canceló

                Usuario? usuario = Sesion.UsuarioActual;
                string quien = usuario == null ? "-" : $"{usuario.Nombre} {usuario.Apellido}";

                GeneradorPdf.Generar(ruta, Titulos[reporte], filtro, quien, tabla);

                Process.Start(new ProcessStartInfo(ruta) { UseShellExecute = true });
            }
            catch (ReglaNegocioException ex)
            {
                Avisar(ex.Message);
            }
            catch (IOException)
            {
                Avisar("No se pudo guardar el archivo. Verifica que no esté abierto en otro programa.");
            }
            catch (Exception ex)
            {
                Avisar("No se pudo generar el PDF.\n" + ex.GetType().FullName + "\n" + ex.Message + "\n\n" + ex.StackTrace);
            }
        }

        private string? PedirRuta(string titulo)
        {
            using (var dialogo = new SaveFileDialog())
            {
                dialogo.Filter = "Archivo PDF (*.pdf)|*.pdf";
                dialogo.FileName = $"Reporte_{titulo.Replace(' ', '_')}_{DateTime.Now:yyyyMMdd_HHmm}.pdf";

                return dialogo.ShowDialog(this) == DialogResult.OK ? dialogo.FileName : null;
            }
        }

        // Convierte la opción elegida en un id numérico (null = "Todas").
        private static int? IdDe(OpcionFiltro? opcion)
        {
            if (opcion?.Valor != null && int.TryParse(opcion.Valor, out int id))
                return id;
            return null;
        }

        private static void Avisar(string mensaje)
        {
            MessageBox.Show(mensaje, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}