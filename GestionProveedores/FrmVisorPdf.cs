using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace GestionProveedores
{
    // Visor de PDF incrustado en la aplicación. Muestra el archivo con el visor del
    // motor de Microsoft Edge (WebView2), sin abrir ningún programa externo.
    public class FrmVisorPdf : Form
    {
        private readonly string _rutaPdf;
        private readonly string _nombreSugerido;
        private readonly WebView2 _visor = new WebView2();

        public FrmVisorPdf(string rutaPdf, string titulo, string nombreSugerido)
        {
            _rutaPdf = rutaPdf;
            _nombreSugerido = nombreSugerido;

            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            Text = titulo;
            StartPosition = FormStartPosition.CenterParent;
            var area = Screen.PrimaryScreen?.WorkingArea ?? new Rectangle(0, 0, 1280, 800);
            ClientSize = new Size((int)(area.Width * 0.8), (int)(area.Height * 0.8));
            StartPosition = FormStartPosition.CenterScreen;
            MinimizeBox = false;

            // Los botones se ajustan solos a su texto y al tamaño de letra de la pantalla.
            var btnCerrar = new Button { Text = "Cerrar", AutoSize = true, Padding = new Padding(12, 4, 12, 4) };
            var btnGuardar = new Button { Text = "Guardar copia…", AutoSize = true, Padding = new Padding(12, 4, 12, 4) };
            btnCerrar.Click += (s, e) => Close();
            btnGuardar.Click += (s, e) => GuardarCopia();

            var panelBotones = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                AutoSize = true,                              // la altura se ajusta a los botones
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(8)
            };
            panelBotones.Controls.Add(btnCerrar);
            panelBotones.Controls.Add(btnGuardar);

            _visor.Dock = DockStyle.Fill;

            // El control que ocupa el resto se agrega primero; el panel de abajo, después.
            Controls.Add(_visor);
            Controls.Add(panelBotones);
        }

        protected override async void OnShown(EventArgs e)
        {
            base.OnShown(e);

            try
            {
                // Carpeta propia para los datos del visor (evita escribir junto al programa).
                string carpetaDatos = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "GestionProveedores", "WebView2");

                var entorno = await CoreWebView2Environment.CreateAsync(null, carpetaDatos);
                await _visor.EnsureCoreWebView2Async(entorno);
                _visor.CoreWebView2.Navigate(new Uri(_rutaPdf).AbsoluteUri);
            }
            catch (WebView2RuntimeNotFoundException)
            {
                MessageBox.Show(
                    "No se encontró el componente 'WebView2 Runtime' de Microsoft, necesario para ver el PDF " +
                    "dentro de la aplicación. Instala el Evergreen Runtime desde la página oficial de WebView2.",
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo mostrar el PDF.\n" + ex.Message,
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
            }
        }

        // Libera el visor para que suelte el archivo antes de que se intente borrar.
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _visor.Dispose();
            base.OnFormClosed(e);
        }

        private void GuardarCopia()
        {
            using (var dialogo = new SaveFileDialog())
            {
                dialogo.Filter = "Archivo PDF (*.pdf)|*.pdf";
                dialogo.FileName = _nombreSugerido;

                if (dialogo.ShowDialog(this) != DialogResult.OK) return;

                try
                {
                    File.Copy(_rutaPdf, dialogo.FileName, true);
                    MessageBox.Show("Copia guardada correctamente.", "Listo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show("No tienes permiso para guardar en esa ubicación.",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch (IOException)
                {
                    MessageBox.Show("No se pudo guardar el archivo. Verifica que no esté abierto en otro programa.",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
    }
}