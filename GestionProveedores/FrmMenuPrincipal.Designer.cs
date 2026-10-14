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
            lblBienvenida = new Label();
            Logout = new Button();
            SuspendLayout();
            // 
            // lblBienvenida
            // 
            lblBienvenida.AutoSize = true;
            lblBienvenida.Font = new Font("Segoe UI", 12F);
            lblBienvenida.Location = new Point(27, 32);
            lblBienvenida.Name = "lblBienvenida";
            lblBienvenida.Size = new Size(109, 28);
            lblBienvenida.TabIndex = 0;
            lblBienvenida.Text = "Bienvenido";
            // 
            // Logout
            // 
            Logout.AccessibleName = "";
            Logout.Location = new Point(558, 28);
            Logout.Name = "Logout";
            Logout.Size = new Size(94, 29);
            Logout.TabIndex = 1;
            Logout.Text = "Logout";
            Logout.UseVisualStyleBackColor = true;
            Logout.Click += mnuCerrarSesion_Click;
            // 
            // FrmMenuPrincipal
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(667, 481);
            Controls.Add(Logout);
            Controls.Add(lblBienvenida);
            Margin = new Padding(3, 4, 3, 4);
            Name = "FrmMenuPrincipal";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Gestión de Proveedores";
            Load += FrmMenuPrincipal_Load;
            FormClosing += FrmMenuPrincipal_FormClosing;
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblBienvenida;
        private Button Logout;
    }
}
