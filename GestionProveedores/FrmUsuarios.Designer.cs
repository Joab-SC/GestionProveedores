namespace GestionProveedores;

partial class FrmUsuarios
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
        dgvUsuarios = new DataGridView();
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
        lblTipoUsuario = new Label();
        cmbTipoUsuario = new ComboBox();
        lblContrasena = new Label();
        txtContrasena = new TextBox();
        chkActivo = new CheckBox();
        btnNuevo = new Button();
        btnGuardar = new Button();
        btnDesactivar = new Button();
        ((System.ComponentModel.ISupportInitialize)dgvUsuarios).BeginInit();
        SuspendLayout();
        // 
        // dgvUsuarios
        // 
        dgvUsuarios.AllowUserToAddRows = false;
        dgvUsuarios.AllowUserToDeleteRows = false;
        dgvUsuarios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvUsuarios.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvUsuarios.Location = new Point(24, 22);
        dgvUsuarios.MultiSelect = false;
        dgvUsuarios.Name = "dgvUsuarios";
        dgvUsuarios.ReadOnly = true;
        dgvUsuarios.RowHeadersWidth = 51;
        dgvUsuarios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvUsuarios.Size = new Size(852, 250);
        dgvUsuarios.TabIndex = 0;
        // 
        // lblCedula
        // 
        lblCedula.AutoSize = true;
        lblCedula.Location = new Point(24, 298);
        lblCedula.Name = "lblCedula";
        lblCedula.Size = new Size(55, 20);
        lblCedula.TabIndex = 1;
        lblCedula.Text = "Cédula";
        // 
        // txtCedula
        // 
        txtCedula.Location = new Point(24, 324);
        txtCedula.MaxLength = 15;
        txtCedula.Name = "txtCedula";
        txtCedula.Size = new Size(192, 27);
        txtCedula.TabIndex = 2;
        // 
        // lblNombre
        // 
        lblNombre.AutoSize = true;
        lblNombre.Location = new Point(232, 298);
        lblNombre.Name = "lblNombre";
        lblNombre.Size = new Size(64, 20);
        lblNombre.TabIndex = 3;
        lblNombre.Text = "Nombre";
        // 
        // txtNombre
        // 
        txtNombre.Location = new Point(232, 324);
        txtNombre.MaxLength = 60;
        txtNombre.Name = "txtNombre";
        txtNombre.Size = new Size(300, 27);
        txtNombre.TabIndex = 4;
        // 
        // lblApellido
        // 
        lblApellido.AutoSize = true;
        lblApellido.Location = new Point(548, 298);
        lblApellido.Name = "lblApellido";
        lblApellido.Size = new Size(65, 20);
        lblApellido.TabIndex = 5;
        lblApellido.Text = "Apellido";
        // 
        // txtApellido
        // 
        txtApellido.Location = new Point(548, 324);
        txtApellido.MaxLength = 60;
        txtApellido.Name = "txtApellido";
        txtApellido.Size = new Size(328, 27);
        txtApellido.TabIndex = 6;
        // 
        // lblFechaNacimiento
        // 
        lblFechaNacimiento.AutoSize = true;
        lblFechaNacimiento.Location = new Point(24, 376);
        lblFechaNacimiento.Name = "lblFechaNacimiento";
        lblFechaNacimiento.Size = new Size(151, 20);
        lblFechaNacimiento.TabIndex = 7;
        lblFechaNacimiento.Text = "Fecha de nacimiento";
        // 
        // dtpFechaNacimiento
        // 
        dtpFechaNacimiento.Format = DateTimePickerFormat.Short;
        dtpFechaNacimiento.Location = new Point(24, 402);
        dtpFechaNacimiento.MinDate = new DateTime(1900, 1, 1, 0, 0, 0, 0);
        dtpFechaNacimiento.Name = "dtpFechaNacimiento";
        dtpFechaNacimiento.Size = new Size(192, 27);
        dtpFechaNacimiento.TabIndex = 8;
        // 
        // lblCiudad
        // 
        lblCiudad.AutoSize = true;
        lblCiudad.Location = new Point(232, 376);
        lblCiudad.Name = "lblCiudad";
        lblCiudad.Size = new Size(58, 20);
        lblCiudad.TabIndex = 9;
        lblCiudad.Text = "Ciudad";
        // 
        // cmbCiudad
        // 
        cmbCiudad.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbCiudad.FormattingEnabled = true;
        cmbCiudad.Location = new Point(232, 402);
        cmbCiudad.Name = "cmbCiudad";
        cmbCiudad.Size = new Size(300, 28);
        cmbCiudad.TabIndex = 10;
        // 
        // lblTipoUsuario
        // 
        lblTipoUsuario.AutoSize = true;
        lblTipoUsuario.Location = new Point(548, 376);
        lblTipoUsuario.Name = "lblTipoUsuario";
        lblTipoUsuario.Size = new Size(115, 20);
        lblTipoUsuario.TabIndex = 11;
        lblTipoUsuario.Text = "Tipo de usuario";
        // 
        // cmbTipoUsuario
        // 
        cmbTipoUsuario.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbTipoUsuario.FormattingEnabled = true;
        cmbTipoUsuario.Location = new Point(548, 402);
        cmbTipoUsuario.Name = "cmbTipoUsuario";
        cmbTipoUsuario.Size = new Size(328, 28);
        cmbTipoUsuario.TabIndex = 12;
        // 
        // lblContrasena
        // 
        lblContrasena.AutoSize = true;
        lblContrasena.Location = new Point(24, 455);
        lblContrasena.Name = "lblContrasena";
        lblContrasena.Size = new Size(92, 20);
        lblContrasena.TabIndex = 13;
        lblContrasena.Text = "Contraseña";
        // 
        // txtContrasena
        // 
        txtContrasena.Location = new Point(24, 481);
        txtContrasena.Name = "txtContrasena";
        txtContrasena.Size = new Size(300, 27);
        txtContrasena.TabIndex = 14;
        txtContrasena.UseSystemPasswordChar = true;
        // 
        // chkActivo
        // 
        chkActivo.AutoSize = true;
        chkActivo.Location = new Point(348, 483);
        chkActivo.Name = "chkActivo";
        chkActivo.Size = new Size(77, 24);
        chkActivo.TabIndex = 15;
        chkActivo.Text = "Activo";
        chkActivo.UseVisualStyleBackColor = true;
        // 
        // btnNuevo
        // 
        btnNuevo.Location = new Point(514, 536);
        btnNuevo.Name = "btnNuevo";
        btnNuevo.Size = new Size(112, 41);
        btnNuevo.TabIndex = 16;
        btnNuevo.Text = "Nuevo";
        btnNuevo.UseVisualStyleBackColor = true;
        // 
        // btnGuardar
        // 
        btnGuardar.Location = new Point(640, 536);
        btnGuardar.Name = "btnGuardar";
        btnGuardar.Size = new Size(112, 41);
        btnGuardar.TabIndex = 17;
        btnGuardar.Text = "Guardar";
        btnGuardar.UseVisualStyleBackColor = true;
        // 
        // btnDesactivar
        // 
        btnDesactivar.Location = new Point(764, 536);
        btnDesactivar.Name = "btnDesactivar";
        btnDesactivar.Size = new Size(112, 41);
        btnDesactivar.TabIndex = 18;
        btnDesactivar.Text = "Desactivar";
        btnDesactivar.UseVisualStyleBackColor = true;
        // 
        // FrmUsuarios
        // 
        AcceptButton = btnGuardar;
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(900, 600);
        Controls.Add(btnDesactivar);
        Controls.Add(btnGuardar);
        Controls.Add(btnNuevo);
        Controls.Add(chkActivo);
        Controls.Add(txtContrasena);
        Controls.Add(lblContrasena);
        Controls.Add(cmbTipoUsuario);
        Controls.Add(lblTipoUsuario);
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
        Controls.Add(dgvUsuarios);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        Name = "FrmUsuarios";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Gestión de usuarios";
        ((System.ComponentModel.ISupportInitialize)dgvUsuarios).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private DataGridView dgvUsuarios;
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
    private Label lblTipoUsuario;
    private ComboBox cmbTipoUsuario;
    private Label lblContrasena;
    private TextBox txtContrasena;
    private CheckBox chkActivo;
    private Button btnNuevo;
    private Button btnGuardar;
    private Button btnDesactivar;
}
