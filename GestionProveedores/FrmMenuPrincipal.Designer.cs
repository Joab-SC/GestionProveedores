namespace GestionProveedores
{
    partial class FrmMenuPrincipal
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el diseñador

        private void InitializeComponent()
        {
            menuPrincipal = new MenuStrip();
            mnuEntidades = new ToolStripMenuItem();
            mnuCiudades = new ToolStripMenuItem();
            mnuCategorias = new ToolStripMenuItem();
            mnuProductos = new ToolStripMenuItem();
            mnuProveedores = new ToolStripMenuItem();
            mnuTransacciones = new ToolStripMenuItem();
            mnuAsignaciones = new ToolStripMenuItem();
            mnuConsultas = new ToolStripMenuItem();
            mnuReportes = new ToolStripMenuItem();
            mnuAuditoria = new ToolStripMenuItem();
            mnuUsuarios = new ToolStripMenuItem();
            mnuBitacora = new ToolStripMenuItem();
            mnuSesion = new ToolStripMenuItem();
            mnuCerrarSesion = new ToolStripMenuItem();
            barraEstado = new StatusStrip();
            lblUsuario = new ToolStripStatusLabel();
            lblBienvenida = new Label();
            menuPrincipal.SuspendLayout();
            barraEstado.SuspendLayout();
            SuspendLayout();
            // 
            // menuPrincipal
            // 
            menuPrincipal.Items.AddRange(new ToolStripItem[] { mnuEntidades, mnuTransacciones, mnuConsultas, mnuReportes, mnuAuditoria, mnuSesion });
            menuPrincipal.Location = new Point(0, 0);
            menuPrincipal.Name = "menuPrincipal";
            menuPrincipal.Size = new Size(667, 28);
            menuPrincipal.TabIndex = 0;
            // 
            // mnuEntidades
            // 
            mnuEntidades.DropDownItems.AddRange(new ToolStripItem[] { mnuCiudades, mnuCategorias, mnuProductos, mnuProveedores });
            mnuEntidades.Name = "mnuEntidades";
            mnuEntidades.Text = "Entidades";
            // 
            // mnuCiudades
            // 
            mnuCiudades.Name = "mnuCiudades";
            mnuCiudades.Text = "Ciudades";
            // 
            // mnuCategorias
            // 
            mnuCategorias.Name = "mnuCategorias";
            mnuCategorias.Text = "Categorías";
            // 
            // mnuProductos
            // 
            mnuProductos.Name = "mnuProductos";
            mnuProductos.Text = "Productos";
            // 
            // mnuProveedores
            // 
            mnuProveedores.Name = "mnuProveedores";
            mnuProveedores.Text = "Proveedores";
            // 
            // mnuTransacciones
            // 
            mnuTransacciones.DropDownItems.AddRange(new ToolStripItem[] { mnuAsignaciones });
            mnuTransacciones.Name = "mnuTransacciones";
            mnuTransacciones.Text = "Transacciones";
            // 
            // mnuAsignaciones
            // 
            mnuAsignaciones.Name = "mnuAsignaciones";
            mnuAsignaciones.Text = "Asignar productos a proveedores";
            // 
            // mnuConsultas
            // 
            mnuConsultas.Name = "mnuConsultas";
            mnuConsultas.Text = "Consultas";
            // 
            // mnuReportes
            // 
            mnuReportes.Name = "mnuReportes";
            mnuReportes.Text = "Reportes";
            // 
            // mnuAuditoria
            // 
            mnuAuditoria.DropDownItems.AddRange(new ToolStripItem[] { mnuUsuarios, mnuBitacora });
            mnuAuditoria.Name = "mnuAuditoria";
            mnuAuditoria.Text = "Auditoría";
            // 
            // mnuUsuarios
            // 
            mnuUsuarios.Name = "mnuUsuarios";
            mnuUsuarios.Text = "Usuarios";
            // 
            // mnuBitacora
            // 
            mnuBitacora.Name = "mnuBitacora";
            mnuBitacora.Text = "Bitácora";
            // 
            // mnuSesion
            // 
            mnuSesion.DropDownItems.AddRange(new ToolStripItem[] { mnuCerrarSesion });
            mnuSesion.Name = "mnuSesion";
            mnuSesion.Text = "Sesión";
            // 
            // mnuCerrarSesion
            // 
            mnuCerrarSesion.Name = "mnuCerrarSesion";
            mnuCerrarSesion.Text = "Cerrar sesión";
            // 
            // barraEstado
            // 
            barraEstado.Items.AddRange(new ToolStripItem[] { lblUsuario });
            barraEstado.Location = new Point(0, 455);
            barraEstado.Name = "barraEstado";
            barraEstado.Size = new Size(667, 26);
            barraEstado.TabIndex = 1;
            // 
            // lblUsuario
            // 
            lblUsuario.Name = "lblUsuario";
            lblUsuario.Text = "Usuario";
            // 
            // lblBienvenida
            // 
            lblBienvenida.AutoSize = true;
            lblBienvenida.Font = new Font("Segoe UI", 12F);
            lblBienvenida.Location = new Point(27, 60);
            lblBienvenida.Name = "lblBienvenida";
            lblBienvenida.Size = new Size(109, 28);
            lblBienvenida.TabIndex = 2;
            lblBienvenida.Text = "Bienvenido";
            // 
            // FrmMenuPrincipal
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(667, 481);
            Controls.Add(lblBienvenida);
            Controls.Add(barraEstado);
            Controls.Add(menuPrincipal);
            MainMenuStrip = menuPrincipal;
            Margin = new Padding(3, 4, 3, 4);
            Name = "FrmMenuPrincipal";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Gestión de Proveedores";
            menuPrincipal.ResumeLayout(false);
            menuPrincipal.PerformLayout();
            barraEstado.ResumeLayout(false);
            barraEstado.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuPrincipal;
        private ToolStripMenuItem mnuEntidades;
        private ToolStripMenuItem mnuCiudades;
        private ToolStripMenuItem mnuCategorias;
        private ToolStripMenuItem mnuProductos;
        private ToolStripMenuItem mnuProveedores;
        private ToolStripMenuItem mnuTransacciones;
        private ToolStripMenuItem mnuAsignaciones;
        private ToolStripMenuItem mnuConsultas;
        private ToolStripMenuItem mnuReportes;
        private ToolStripMenuItem mnuAuditoria;
        private ToolStripMenuItem mnuUsuarios;
        private ToolStripMenuItem mnuBitacora;
        private ToolStripMenuItem mnuSesion;
        private ToolStripMenuItem mnuCerrarSesion;
        private StatusStrip barraEstado;
        private ToolStripStatusLabel lblUsuario;
        private Label lblBienvenida;
    }
}