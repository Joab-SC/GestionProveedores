namespace GestionProveedores;

partial class FrmBitacora
{
    private System.ComponentModel.IContainer? components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        lblUsuario = new Label();
        cmbUsuario = new ComboBox();
        lblDesde = new Label();
        dtpDesde = new DateTimePicker();
        lblHasta = new Label();
        dtpHasta = new DateTimePicker();
        btnFiltrar = new Button();
        dgvBitacora = new DataGridView();
        ((System.ComponentModel.ISupportInitialize)dgvBitacora).BeginInit();
        SuspendLayout();
        // 
        // lblUsuario
        // 
        lblUsuario.AutoSize = true;
        lblUsuario.Location = new Point(24, 23);
        lblUsuario.Name = "lblUsuario";
        lblUsuario.Size = new Size(59, 20);
        lblUsuario.TabIndex = 0;
        lblUsuario.Text = "Usuario";
        // 
        // cmbUsuario
        // 
        cmbUsuario.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbUsuario.FormattingEnabled = true;
        cmbUsuario.Location = new Point(24, 49);
        cmbUsuario.Name = "cmbUsuario";
        cmbUsuario.Size = new Size(300, 28);
        cmbUsuario.TabIndex = 1;
        // 
        // lblDesde
        // 
        lblDesde.AutoSize = true;
        lblDesde.Location = new Point(340, 23);
        lblDesde.Name = "lblDesde";
        lblDesde.Size = new Size(51, 20);
        lblDesde.TabIndex = 2;
        lblDesde.Text = "Desde";
        // 
        // dtpDesde
        // 
        dtpDesde.Format = DateTimePickerFormat.Short;
        dtpDesde.Location = new Point(340, 49);
        dtpDesde.Name = "dtpDesde";
        dtpDesde.Size = new Size(140, 27);
        dtpDesde.TabIndex = 3;
        // 
        // lblHasta
        // 
        lblHasta.AutoSize = true;
        lblHasta.Location = new Point(496, 23);
        lblHasta.Name = "lblHasta";
        lblHasta.Size = new Size(47, 20);
        lblHasta.TabIndex = 4;
        lblHasta.Text = "Hasta";
        // 
        // dtpHasta
        // 
        dtpHasta.Format = DateTimePickerFormat.Short;
        dtpHasta.Location = new Point(496, 49);
        dtpHasta.Name = "dtpHasta";
        dtpHasta.Size = new Size(140, 27);
        dtpHasta.TabIndex = 5;
        // 
        // btnFiltrar
        // 
        btnFiltrar.Location = new Point(652, 47);
        btnFiltrar.Name = "btnFiltrar";
        btnFiltrar.Size = new Size(112, 31);
        btnFiltrar.TabIndex = 6;
        btnFiltrar.Text = "Filtrar";
        btnFiltrar.UseVisualStyleBackColor = true;
        btnFiltrar.Click += btnFiltrar_Click_1;
        // 
        // dgvBitacora
        // 
        dgvBitacora.AllowUserToAddRows = false;
        dgvBitacora.AllowUserToDeleteRows = false;
        dgvBitacora.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvBitacora.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvBitacora.Location = new Point(24, 101);
        dgvBitacora.MultiSelect = false;
        dgvBitacora.Name = "dgvBitacora";
        dgvBitacora.ReadOnly = true;
        dgvBitacora.RowHeadersWidth = 51;
        dgvBitacora.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvBitacora.Size = new Size(740, 346);
        dgvBitacora.TabIndex = 7;
        // 
        // FrmBitacora
        // 
        AcceptButton = btnFiltrar;
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(790, 470);
        Controls.Add(dgvBitacora);
        Controls.Add(btnFiltrar);
        Controls.Add(dtpHasta);
        Controls.Add(lblHasta);
        Controls.Add(dtpDesde);
        Controls.Add(lblDesde);
        Controls.Add(cmbUsuario);
        Controls.Add(lblUsuario);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        Name = "FrmBitacora";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Bitácora";
        ((System.ComponentModel.ISupportInitialize)dgvBitacora).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    private Label lblUsuario;
    private ComboBox cmbUsuario;
    private Label lblDesde;
    private DateTimePicker dtpDesde;
    private Label lblHasta;
    private DateTimePicker dtpHasta;
    private Button btnFiltrar;
    private DataGridView dgvBitacora;
}
