namespace GestionProveedores
{
    partial class FrmReportes
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

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            lblReporte = new Label();
            cmbReporte = new ComboBox();
            lblFiltro = new Label();
            cmbFiltro = new ComboBox();
            lblTexto = new Label();
            txtFiltro = new TextBox();
            btnGenerar = new Button();
            SuspendLayout();
            // 
            // lblReporte
            // 
            lblReporte.AutoSize = true;
            lblReporte.Location = new Point(20, 20);
            lblReporte.Name = "lblReporte";
            lblReporte.Size = new Size(48, 15);
            lblReporte.TabIndex = 10;
            lblReporte.Text = "Reporte";
            // 
            // cmbReporte
            // 
            cmbReporte.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbReporte.FormattingEnabled = true;
            cmbReporte.Location = new Point(20, 42);
            cmbReporte.Name = "cmbReporte";
            cmbReporte.Size = new Size(500, 23);
            cmbReporte.TabIndex = 0;
            // 
            // lblFiltro
            // 
            lblFiltro.AutoSize = true;
            lblFiltro.Location = new Point(20, 92);
            lblFiltro.Name = "lblFiltro";
            lblFiltro.Size = new Size(35, 15);
            lblFiltro.TabIndex = 11;
            lblFiltro.Text = "Filtro";
            // 
            // cmbFiltro
            // 
            cmbFiltro.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFiltro.FormattingEnabled = true;
            cmbFiltro.Location = new Point(20, 114);
            cmbFiltro.Name = "cmbFiltro";
            cmbFiltro.Size = new Size(500, 23);
            cmbFiltro.TabIndex = 1;
            // 
            // lblTexto
            // 
            lblTexto.AutoSize = true;
            lblTexto.Location = new Point(20, 164);
            lblTexto.Name = "lblTexto";
            lblTexto.Size = new Size(170, 15);
            lblTexto.TabIndex = 12;
            lblTexto.Text = "Texto a buscar en el nombre";
            // 
            // txtFiltro
            // 
            txtFiltro.Location = new Point(20, 186);
            txtFiltro.MaxLength = 60;
            txtFiltro.Name = "txtFiltro";
            txtFiltro.Size = new Size(500, 23);
            txtFiltro.TabIndex = 2;
            // 
            // btnGenerar
            // 
            btnGenerar.Location = new Point(20, 236);
            btnGenerar.Name = "btnGenerar";
            btnGenerar.Size = new Size(160, 36);
            btnGenerar.TabIndex = 3;
            btnGenerar.Text = "Generar PDF";
            btnGenerar.UseVisualStyleBackColor = true;
            // 
            // FrmReportes
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(540, 296);
            Controls.Add(btnGenerar);
            Controls.Add(txtFiltro);
            Controls.Add(lblTexto);
            Controls.Add(cmbFiltro);
            Controls.Add(lblFiltro);
            Controls.Add(cmbReporte);
            Controls.Add(lblReporte);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "FrmReportes";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Reportes";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblReporte;
        private ComboBox cmbReporte;
        private Label lblFiltro;
        private ComboBox cmbFiltro;
        private Label lblTexto;
        private TextBox txtFiltro;
        private Button btnGenerar;
    }
}