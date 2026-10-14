namespace GestionProveedores
{
    partial class FrmCategorias
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
            dgvCategorias = new DataGridView();
            lblNombre = new Label();
            txtNombre = new TextBox();
            lblDescripcion = new Label();
            txtDescripcion = new TextBox();
            btnNuevo = new Button();
            btnGuardar = new Button();
            btnEliminar = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvCategorias).BeginInit();
            SuspendLayout();
            // 
            // dgvCategorias
            // 
            dgvCategorias.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvCategorias.Location = new Point(22, 26);
            dgvCategorias.Margin = new Padding(6);
            dgvCategorias.Name = "dgvCategorias";
            dgvCategorias.RowHeadersWidth = 82;
            dgvCategorias.Size = new Size(1367, 555);
            dgvCategorias.TabIndex = 0;
            // 
            // lblNombre
            // 
            lblNombre.AutoSize = true;
            lblNombre.Location = new Point(22, 619);
            lblNombre.Margin = new Padding(6, 0, 6, 0);
            lblNombre.Name = "lblNombre";
            lblNombre.Size = new Size(102, 32);
            lblNombre.TabIndex = 7;
            lblNombre.Text = "Nombre";
            // 
            // txtNombre
            // 
            txtNombre.Location = new Point(22, 666);
            txtNombre.Margin = new Padding(6);
            txtNombre.MaxLength = 60;
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new Size(665, 39);
            txtNombre.TabIndex = 1;
            // 
            // lblDescripcion
            // 
            lblDescripcion.AutoSize = true;
            lblDescripcion.Location = new Point(22, 742);
            lblDescripcion.Margin = new Padding(6, 0, 6, 0);
            lblDescripcion.Name = "lblDescripcion";
            lblDescripcion.Size = new Size(138, 32);
            lblDescripcion.TabIndex = 6;
            lblDescripcion.Text = "Descripción";
            // 
            // txtDescripcion
            // 
            txtDescripcion.Location = new Point(22, 789);
            txtDescripcion.Margin = new Padding(6);
            txtDescripcion.MaxLength = 250;
            txtDescripcion.Multiline = true;
            txtDescripcion.Name = "txtDescripcion";
            txtDescripcion.Size = new Size(1363, 166);
            txtDescripcion.TabIndex = 2;
            // 
            // btnNuevo
            // 
            btnNuevo.Location = new Point(22, 994);
            btnNuevo.Margin = new Padding(6);
            btnNuevo.Name = "btnNuevo";
            btnNuevo.Size = new Size(223, 73);
            btnNuevo.TabIndex = 3;
            btnNuevo.Text = "Limpiar";
            btnNuevo.UseVisualStyleBackColor = true;
            // 
            // btnGuardar
            // 
            btnGuardar.Location = new Point(264, 994);
            btnGuardar.Margin = new Padding(6);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(223, 73);
            btnGuardar.TabIndex = 4;
            btnGuardar.Text = "Guardar";
            btnGuardar.UseVisualStyleBackColor = true;
            // 
            // btnEliminar
            // 
            btnEliminar.Location = new Point(505, 994);
            btnEliminar.Margin = new Padding(6);
            btnEliminar.Name = "btnEliminar";
            btnEliminar.Size = new Size(223, 73);
            btnEliminar.TabIndex = 5;
            btnEliminar.Text = "Eliminar";
            btnEliminar.UseVisualStyleBackColor = true;
            // 
            // FrmCategorias
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1411, 1101);
            Controls.Add(btnEliminar);
            Controls.Add(btnGuardar);
            Controls.Add(btnNuevo);
            Controls.Add(txtDescripcion);
            Controls.Add(lblDescripcion);
            Controls.Add(txtNombre);
            Controls.Add(lblNombre);
            Controls.Add(dgvCategorias);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(6);
            MaximizeBox = false;
            Name = "FrmCategorias";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Categorías";
            ((System.ComponentModel.ISupportInitialize)dgvCategorias).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dgvCategorias;
        private Label lblNombre;
        private TextBox txtNombre;
        private Label lblDescripcion;
        private TextBox txtDescripcion;
        private Button btnNuevo;
        private Button btnGuardar;
        private Button btnEliminar;
    }
}