namespace GestionProveedores;

partial class FrmAsignaciones
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
        lblProveedor = new Label();
        cmbProveedor = new ComboBox();
        lblDisponibles = new Label();
        lblAsignados = new Label();
        lstDisponibles = new ListBox();
        lstAsignados = new ListBox();
        btnAgregar = new Button();
        btnQuitar = new Button();
        btnGuardarCambios = new Button();
        SuspendLayout();
        // 
        // lblProveedor
        // 
        lblProveedor.AutoSize = true;
        lblProveedor.Location = new Point(24, 24);
        lblProveedor.Name = "lblProveedor";
        lblProveedor.Size = new Size(71, 20);
        lblProveedor.TabIndex = 0;
        lblProveedor.Text = "Proveedor";
        // 
        // cmbProveedor
        // 
        cmbProveedor.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbProveedor.FormattingEnabled = true;
        cmbProveedor.Location = new Point(24, 50);
        cmbProveedor.Name = "cmbProveedor";
        cmbProveedor.Size = new Size(552, 28);
        cmbProveedor.TabIndex = 1;
        // 
        // lblDisponibles
        // 
        lblDisponibles.AutoSize = true;
        lblDisponibles.Location = new Point(24, 104);
        lblDisponibles.Name = "lblDisponibles";
        lblDisponibles.Size = new Size(207, 20);
        lblDisponibles.TabIndex = 2;
        lblDisponibles.Text = "Productos disponibles para asignar";
        // 
        // lblAsignados
        // 
        lblAsignados.AutoSize = true;
        lblAsignados.Location = new Point(388, 104);
        lblAsignados.Name = "lblAsignados";
        lblAsignados.Size = new Size(153, 20);
        lblAsignados.TabIndex = 3;
        lblAsignados.Text = "Productos asignados";
        // 
        // lstDisponibles
        // 
        lstDisponibles.FormattingEnabled = true;
        lstDisponibles.IntegralHeight = false;
        lstDisponibles.Location = new Point(24, 132);
        lstDisponibles.Name = "lstDisponibles";
        lstDisponibles.Size = new Size(280, 286);
        lstDisponibles.TabIndex = 4;
        // 
        // lstAsignados
        // 
        lstAsignados.FormattingEnabled = true;
        lstAsignados.IntegralHeight = false;
        lstAsignados.Location = new Point(388, 132);
        lstAsignados.Name = "lstAsignados";
        lstAsignados.Size = new Size(280, 286);
        lstAsignados.TabIndex = 7;
        // 
        // btnAgregar
        // 
        btnAgregar.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        btnAgregar.Location = new Point(322, 205);
        btnAgregar.Name = "btnAgregar";
        btnAgregar.Size = new Size(48, 42);
        btnAgregar.TabIndex = 5;
        btnAgregar.Text = ">";
        btnAgregar.UseVisualStyleBackColor = true;
        // 
        // btnQuitar
        // 
        btnQuitar.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        btnQuitar.Location = new Point(322, 270);
        btnQuitar.Name = "btnQuitar";
        btnQuitar.Size = new Size(48, 42);
        btnQuitar.TabIndex = 6;
        btnQuitar.Text = "<";
        btnQuitar.UseVisualStyleBackColor = true;
        // 
        // btnGuardarCambios
        // 
        btnGuardarCambios.Location = new Point(493, 442);
        btnGuardarCambios.Name = "btnGuardarCambios";
        btnGuardarCambios.Size = new Size(175, 42);
        btnGuardarCambios.TabIndex = 8;
        btnGuardarCambios.Text = "Guardar cambios";
        btnGuardarCambios.UseVisualStyleBackColor = true;
        // 
        // FrmAsignaciones
        // 
        AcceptButton = btnGuardarCambios;
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(692, 507);
        Controls.Add(btnGuardarCambios);
        Controls.Add(btnQuitar);
        Controls.Add(btnAgregar);
        Controls.Add(lstAsignados);
        Controls.Add(lstDisponibles);
        Controls.Add(lblAsignados);
        Controls.Add(lblDisponibles);
        Controls.Add(cmbProveedor);
        Controls.Add(lblProveedor);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "FrmAsignaciones";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Asignar productos a proveedores";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblProveedor;
    private ComboBox cmbProveedor;
    private Label lblDisponibles;
    private Label lblAsignados;
    private ListBox lstDisponibles;
    private ListBox lstAsignados;
    private Button btnAgregar;
    private Button btnQuitar;
    private Button btnGuardarCambios;
}
