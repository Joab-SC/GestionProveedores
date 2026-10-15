namespace GestionProveedores;

partial class FrmConsultas
{
    private System.ComponentModel.IContainer? components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            components?.Dispose();
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        lblConsulta = new Label();
        cmbConsulta = new ComboBox();
        lblDescripcionTitulo = new Label();
        lblDescripcion = new Label();
        btnEjecutar = new Button();
        dgvResultado = new DataGridView();
        lblResultado = new Label();
        ((System.ComponentModel.ISupportInitialize)dgvResultado).BeginInit();
        SuspendLayout();
        // 
        // lblConsulta
        // 
        lblConsulta.AutoSize = true;
        lblConsulta.Location = new Point(24, 22);
        lblConsulta.Name = "lblConsulta";
        lblConsulta.Size = new Size(67, 20);
        lblConsulta.TabIndex = 0;
        lblConsulta.Text = "Consulta";
        // 
        // cmbConsulta
        // 
        cmbConsulta.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbConsulta.FormattingEnabled = true;
        cmbConsulta.Location = new Point(24, 48);
        cmbConsulta.Name = "cmbConsulta";
        cmbConsulta.Size = new Size(627, 28);
        cmbConsulta.TabIndex = 1;
        // 
        // lblDescripcionTitulo
        // 
        lblDescripcionTitulo.AutoSize = true;
        lblDescripcionTitulo.Location = new Point(24, 101);
        lblDescripcionTitulo.Name = "lblDescripcionTitulo";
        lblDescripcionTitulo.Size = new Size(91, 20);
        lblDescripcionTitulo.TabIndex = 2;
        lblDescripcionTitulo.Text = "Descripción";
        // 
        // lblDescripcion
        // 
        lblDescripcion.BorderStyle = BorderStyle.FixedSingle;
        lblDescripcion.Location = new Point(24, 128);
        lblDescripcion.Name = "lblDescripcion";
        lblDescripcion.Padding = new Padding(8);
        lblDescripcion.Size = new Size(627, 62);
        lblDescripcion.TabIndex = 3;
        // 
        // btnEjecutar
        // 
        btnEjecutar.Location = new Point(505, 206);
        btnEjecutar.Name = "btnEjecutar";
        btnEjecutar.Size = new Size(146, 39);
        btnEjecutar.TabIndex = 4;
        btnEjecutar.Text = "Ejecutar consulta";
        btnEjecutar.UseVisualStyleBackColor = true;
        // 
        // dgvResultado
        // 
        dgvResultado.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvResultado.Location = new Point(24, 292);
        dgvResultado.Name = "dgvResultado";
        dgvResultado.RowHeadersWidth = 51;
        dgvResultado.Size = new Size(852, 324);
        dgvResultado.TabIndex = 6;
        // 
        // lblResultado
        // 
        lblResultado.AutoSize = true;
        lblResultado.Location = new Point(24, 264);
        lblResultado.Name = "lblResultado";
        lblResultado.Size = new Size(79, 20);
        lblResultado.TabIndex = 5;
        lblResultado.Text = "Resultado";
        // 
        // FrmConsultas
        // 
        AcceptButton = btnEjecutar;
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(900, 641);
        Controls.Add(dgvResultado);
        Controls.Add(lblResultado);
        Controls.Add(btnEjecutar);
        Controls.Add(lblDescripcion);
        Controls.Add(lblDescripcionTitulo);
        Controls.Add(cmbConsulta);
        Controls.Add(lblConsulta);
        MinimumSize = new Size(918, 688);
        Name = "FrmConsultas";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Consultas";
        ((System.ComponentModel.ISupportInitialize)dgvResultado).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblConsulta;
    private ComboBox cmbConsulta;
    private Label lblDescripcionTitulo;
    private Label lblDescripcion;
    private Button btnEjecutar;
    private DataGridView dgvResultado;
    private Label lblResultado;
}
