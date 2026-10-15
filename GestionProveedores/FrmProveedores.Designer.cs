namespace GestionProveedores
{
    partial class FrmProveedores
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
            dgvProveedores = new DataGridView();
            lblCedula = new Label();
            txtCedula = new TextBox();
            lblNombre = new Label();
            txtNombre = new TextBox();
            lblApellido = new Label();
            txtApellido = new TextBox();
            lblFechaNacimiento = new Label();
            dtpFechaNacimiento = new DateTimePicker();
            lblCiudad = new Label();
            cmbCiudad = new ComboBox();
            btnNuevo = new Button();
            btnGuardar = new Button();
            btnEliminar = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvProveedores).BeginInit();
            SuspendLayout();
            // 
            // dgvProveedores
            // 
            dgvProveedores.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvProveedores.Location = new Point(12, 12);
            dgvProveedores.Name = "dgvProveedores";
            dgvProveedores.RowHeadersWidth = 82;
            dgvProveedores.Size = new Size(736, 230);
            dgvProveedores.TabIndex = 0;
            // 
            // lblCedula
            // 
            lblCedula.AutoSize = true;
            lblCedula.Location = new Point(12, 256);
            lblCedula.Name = "lblCedula";
            lblCedula.Size = new Size(46, 15);
            lblCedula.TabIndex = 20;
            lblCedula.Text = "Cédula";
            // 
            // txtCedula
            // 
            txtCedula.Location = new Point(12, 278);
            txtCedula.MaxLength = 15;
            txtCedula.Name = "txtCedula";
            txtCedula.Size = new Size(200, 23);
            txtCedula.TabIndex = 1;
            // 
            // lblNombre
            // 
            lblNombre.AutoSize = true;
            lblNombre.Location = new Point(232, 256);
            lblNombre.Name = "lblNombre";
            lblNombre.Size = new Size(51, 15);
            lblNombre.TabIndex = 21;
            lblNombre.Text = "Nombre";
            // 
            // txtNombre
            // 
            txtNombre.Location = new Point(232, 278);
            txtNombre.MaxLength = 60;
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new Size(240, 23);
            txtNombre.TabIndex = 2;
            // 
            // lblApellido
            // 
            lblApellido.AutoSize = true;
            lblApellido.Location = new Point(492, 256);
            lblApellido.Name = "lblApellido";
            lblApellido.Size = new Size(51, 15);
            lblApellido.TabIndex = 22;
            lblApellido.Text = "Apellido";
            // 
            // txtApellido
            // 
            txtApellido.Location = new Point(492, 278);
            txtApellido.MaxLength = 60;
            txtApellido.Name = "txtApellido";
            txtApellido.Size = new Size(256, 23);
            txtApellido.TabIndex = 3;
            // 
            // lblFechaNacimiento
            // 
            lblFechaNacimiento.AutoSize = true;
            lblFechaNacimiento.Location = new Point(12, 318);
            lblFechaNacimiento.Name = "lblFechaNacimiento";
            lblFechaNacimiento.Size = new Size(121, 15);
            lblFechaNacimiento.TabIndex = 23;
            lblFechaNacimiento.Text = "Fecha de nacimiento";
            // 
            // dtpFechaNacimiento
            // 
            dtpFechaNacimiento.Format = DateTimePickerFormat.Short;
            dtpFechaNacimiento.Location = new Point(12, 340);
            dtpFechaNacimiento.Name = "dtpFechaNacimiento";
            dtpFechaNacimiento.Size = new Size(200, 23);
            dtpFechaNacimiento.TabIndex = 4;
            // 
            // lblCiudad
            // 
            lblCiudad.AutoSize = true;
            lblCiudad.Location = new Point(232, 318);
            lblCiudad.Name = "lblCiudad";
            lblCiudad.Size = new Size(44, 15);
            lblCiudad.TabIndex = 24;
            lblCiudad.Text = "Ciudad";
            // 
            // cmbCiudad
            // 
            cmbCiudad.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCiudad.FormattingEnabled = true;
            cmbCiudad.Location = new Point(232, 340);
            cmbCiudad.Name = "cmbCiudad";
            cmbCiudad.Size = new Size(240, 23);
            cmbCiudad.TabIndex = 5;
            // 
            // btnNuevo
            // 
            btnNuevo.Location = new Point(12, 392);
            btnNuevo.Name = "btnNuevo";
            btnNuevo.Size = new Size(120, 34);
            btnNuevo.TabIndex = 6;
            btnNuevo.Text = "Nuevo";
            btnNuevo.UseVisualStyleBackColor = true;
            // 
            // btnGuardar
            // 
            btnGuardar.Location = new Point(142, 392);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(120, 34);
            btnGuardar.TabIndex = 7;
            btnGuardar.Text = "Guardar";
            btnGuardar.UseVisualStyleBackColor = true;
            // 
            // btnEliminar
            // 
            btnEliminar.Location = new Point(272, 392);
            btnEliminar.Name = "btnEliminar";
            btnEliminar.Size = new Size(120, 34);
            btnEliminar.TabIndex = 8;
            btnEliminar.Text = "Eliminar";
            btnEliminar.UseVisualStyleBackColor = true;
            // 
            // FrmProveedores
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(760, 446);
            Controls.Add(btnEliminar);
            Controls.Add(btnGuardar);
            Controls.Add(btnNuevo);
            Controls.Add(cmbCiudad);
            Controls.Add(lblCiudad);
            Controls.Add(dtpFechaNacimiento);
            Controls.Add(lblFechaNacimiento);
            Controls.Add(txtApellido);
            Controls.Add(lblApellido);
            Controls.Add(txtNombre);
            Controls.Add(lblNombre);
            Controls.Add(txtCedula);
            Controls.Add(lblCedula);
            Controls.Add(dgvProveedores);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "FrmProveedores";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Proveedores";
            ((System.ComponentModel.ISupportInitialize)dgvProveedores).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dgvProveedores;
        private Label lblCedula;
        private TextBox txtCedula;
        private Label lblNombre;
        private TextBox txtNombre;
        private Label lblApellido;
        private TextBox txtApellido;
        private Label lblFechaNacimiento;
        private DateTimePicker dtpFechaNacimiento;
        private Label lblCiudad;
        private ComboBox cmbCiudad;
        private Button btnNuevo;
        private Button btnGuardar;
        private Button btnEliminar;
    }
}