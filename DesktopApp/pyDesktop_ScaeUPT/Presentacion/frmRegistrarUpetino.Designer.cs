namespace SCAE_UPT.Presentacion
{
    partial class frmRegistrarUpetino
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.btnRetroceder = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.lblMensajeRostro = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.pcbFotoCapturada = new System.Windows.Forms.PictureBox();
            this.btnApagarCamara = new System.Windows.Forms.Button();
            this.btnCapturarFoto = new System.Windows.Forms.Button();
            this.btnPrenderCamara = new System.Windows.Forms.Button();
            this.btnCargarCamaras = new System.Windows.Forms.Button();
            this.cmbCamaras = new System.Windows.Forms.ComboBox();
            this.pcbCamara = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.pcbFoto = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtApellido = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.btnEscanearQR = new System.Windows.Forms.Button();
            this.dgvHistorialRegistroUpetino = new System.Windows.Forms.DataGridView();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pcbFotoCapturada)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pcbCamara)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pcbFoto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistorialRegistroUpetino)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.btnRetroceder);
            this.panel1.Location = new System.Drawing.Point(16, 15);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1449, 80);
            this.panel1.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(500, 18);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(320, 45);
            this.label1.TabIndex = 1;
            this.label1.Text = "REGISTRO UPETINO";
            // 
            // btnRetroceder
            // 
            this.btnRetroceder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.btnRetroceder.FlatAppearance.BorderSize = 0;
            this.btnRetroceder.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.btnRetroceder.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(134)))), ((int)(((byte)(193)))));
            this.btnRetroceder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRetroceder.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnRetroceder.ForeColor = System.Drawing.Color.White;
            this.btnRetroceder.Location = new System.Drawing.Point(20, 25);
            this.btnRetroceder.Margin = new System.Windows.Forms.Padding(4);
            this.btnRetroceder.Name = "btnRetroceder";
            this.btnRetroceder.Size = new System.Drawing.Size(120, 35);
            this.btnRetroceder.TabIndex = 0;
            this.btnRetroceder.Text = "← Retroceder";
            this.btnRetroceder.UseVisualStyleBackColor = false;
            this.btnRetroceder.Click += new System.EventHandler(this.btnRetroceder_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(240)))), ((int)(((byte)(241)))));
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.lblMensajeRostro);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.pcbFotoCapturada);
            this.panel2.Controls.Add(this.btnApagarCamara);
            this.panel2.Controls.Add(this.btnCapturarFoto);
            this.panel2.Controls.Add(this.btnPrenderCamara);
            this.panel2.Controls.Add(this.btnCargarCamaras);
            this.panel2.Controls.Add(this.cmbCamaras);
            this.panel2.Controls.Add(this.pcbCamara);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.pcbFoto);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.txtApellido);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.txtNombre);
            this.panel2.Controls.Add(this.btnEscanearQR);
            this.panel2.Controls.Add(this.dgvHistorialRegistroUpetino);
            this.panel2.Location = new System.Drawing.Point(16, 102);
            this.panel2.Margin = new System.Windows.Forms.Padding(4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1578, 1076);
            this.panel2.TabIndex = 7;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(62)))), ((int)(((byte)(80)))));
            this.label6.Location = new System.Drawing.Point(161, 663);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(140, 21);
            this.label6.TabIndex = 20;
            this.label6.Text = "Últimos registros";
            // 
            // lblMensajeRostro
            // 
            this.lblMensajeRostro.AutoSize = true;
            this.lblMensajeRostro.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMensajeRostro.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.lblMensajeRostro.Location = new System.Drawing.Point(507, 611);
            this.lblMensajeRostro.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMensajeRostro.Name = "lblMensajeRostro";
            this.lblMensajeRostro.Size = new System.Drawing.Size(313, 25);
            this.lblMensajeRostro.TabIndex = 19;
            this.lblMensajeRostro.Text = "Mensaje sobre Alineación de Rostro";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(62)))), ((int)(((byte)(80)))));
            this.label5.Location = new System.Drawing.Point(1105, 325);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(117, 20);
            this.label5.TabIndex = 18;
            this.label5.Text = "Foto Capturada";
            // 
            // pcbFotoCapturada
            // 
            this.pcbFotoCapturada.BackColor = System.Drawing.Color.White;
            this.pcbFotoCapturada.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pcbFotoCapturada.Location = new System.Drawing.Point(1109, 358);
            this.pcbFotoCapturada.Margin = new System.Windows.Forms.Padding(4);
            this.pcbFotoCapturada.Name = "pcbFotoCapturada";
            this.pcbFotoCapturada.Size = new System.Drawing.Size(292, 234);
            this.pcbFotoCapturada.TabIndex = 17;
            this.pcbFotoCapturada.TabStop = false;
            // 
            // btnApagarCamara
            // 
            this.btnApagarCamara.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.btnApagarCamara.FlatAppearance.BorderSize = 0;
            this.btnApagarCamara.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(57)))), ((int)(((byte)(43)))));
            this.btnApagarCamara.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(84)))), ((int)(((byte)(0)))));
            this.btnApagarCamara.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApagarCamara.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnApagarCamara.ForeColor = System.Drawing.Color.White;
            this.btnApagarCamara.Location = new System.Drawing.Point(44, 358);
            this.btnApagarCamara.Margin = new System.Windows.Forms.Padding(4);
            this.btnApagarCamara.Name = "btnApagarCamara";
            this.btnApagarCamara.Size = new System.Drawing.Size(155, 35);
            this.btnApagarCamara.TabIndex = 16;
            this.btnApagarCamara.Text = "🔴 Apagar Cámara";
            this.btnApagarCamara.UseVisualStyleBackColor = false;
            this.btnApagarCamara.Click += new System.EventHandler(this.btnApagarCamara_Click);
            // 
            // btnCapturarFoto
            // 
            this.btnCapturarFoto.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.btnCapturarFoto.FlatAppearance.BorderSize = 0;
            this.btnCapturarFoto.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(174)))), ((int)(((byte)(96)))));
            this.btnCapturarFoto.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(180)))), ((int)(((byte)(99)))));
            this.btnCapturarFoto.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCapturarFoto.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnCapturarFoto.ForeColor = System.Drawing.Color.White;
            this.btnCapturarFoto.Location = new System.Drawing.Point(44, 299);
            this.btnCapturarFoto.Margin = new System.Windows.Forms.Padding(4);
            this.btnCapturarFoto.Name = "btnCapturarFoto";
            this.btnCapturarFoto.Size = new System.Drawing.Size(155, 35);
            this.btnCapturarFoto.TabIndex = 15;
            this.btnCapturarFoto.Text = "📸 Capturar";
            this.btnCapturarFoto.UseVisualStyleBackColor = false;
            this.btnCapturarFoto.Click += new System.EventHandler(this.btnCapturarFoto_Click);
            // 
            // btnPrenderCamara
            // 
            this.btnPrenderCamara.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.btnPrenderCamara.FlatAppearance.BorderSize = 0;
            this.btnPrenderCamara.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.btnPrenderCamara.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(134)))), ((int)(((byte)(193)))));
            this.btnPrenderCamara.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrenderCamara.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnPrenderCamara.ForeColor = System.Drawing.Color.White;
            this.btnPrenderCamara.Location = new System.Drawing.Point(44, 245);
            this.btnPrenderCamara.Margin = new System.Windows.Forms.Padding(4);
            this.btnPrenderCamara.Name = "btnPrenderCamara";
            this.btnPrenderCamara.Size = new System.Drawing.Size(156, 35);
            this.btnPrenderCamara.TabIndex = 14;
            this.btnPrenderCamara.Text = "🎥 Prender Cámara";
            this.btnPrenderCamara.UseVisualStyleBackColor = false;
            this.btnPrenderCamara.Click += new System.EventHandler(this.btnPrenderCamara_Click);
            // 
            // btnCargarCamaras
            // 
            this.btnCargarCamaras.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(89)))), ((int)(((byte)(182)))));
            this.btnCargarCamaras.FlatAppearance.BorderSize = 0;
            this.btnCargarCamaras.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(68)))), ((int)(((byte)(173)))));
            this.btnCargarCamaras.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(79)))), ((int)(((byte)(177)))));
            this.btnCargarCamaras.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCargarCamaras.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnCargarCamaras.ForeColor = System.Drawing.Color.White;
            this.btnCargarCamaras.Location = new System.Drawing.Point(44, 91);
            this.btnCargarCamaras.Margin = new System.Windows.Forms.Padding(4);
            this.btnCargarCamaras.Name = "btnCargarCamaras";
            this.btnCargarCamaras.Size = new System.Drawing.Size(156, 35);
            this.btnCargarCamaras.TabIndex = 13;
            this.btnCargarCamaras.Text = "🔍 Cargar Cámaras";
            this.btnCargarCamaras.UseVisualStyleBackColor = false;
            this.btnCargarCamaras.Click += new System.EventHandler(this.btnCargarCamaras_Click);
            // 
            // cmbCamaras
            // 
            this.cmbCamaras.BackColor = System.Drawing.Color.White;
            this.cmbCamaras.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCamaras.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbCamaras.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(62)))), ((int)(((byte)(80)))));
            this.cmbCamaras.FormattingEnabled = true;
            this.cmbCamaras.Location = new System.Drawing.Point(44, 140);
            this.cmbCamaras.Margin = new System.Windows.Forms.Padding(4);
            this.cmbCamaras.Name = "cmbCamaras";
            this.cmbCamaras.Size = new System.Drawing.Size(153, 23);
            this.cmbCamaras.TabIndex = 12;
            // 
            // pcbCamara
            // 
            this.pcbCamara.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.pcbCamara.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pcbCamara.Location = new System.Drawing.Point(307, 91);
            this.pcbCamara.Margin = new System.Windows.Forms.Padding(4);
            this.pcbCamara.Name = "pcbCamara";
            this.pcbCamara.Size = new System.Drawing.Size(775, 501);
            this.pcbCamara.TabIndex = 11;
            this.pcbCamara.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(62)))), ((int)(((byte)(80)))));
            this.label2.Location = new System.Drawing.Point(1105, 20);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 20);
            this.label2.TabIndex = 10;
            this.label2.Text = "Foto";
            // 
            // pcbFoto
            // 
            this.pcbFoto.BackColor = System.Drawing.Color.White;
            this.pcbFoto.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pcbFoto.Location = new System.Drawing.Point(1109, 57);
            this.pcbFoto.Margin = new System.Windows.Forms.Padding(4);
            this.pcbFoto.Name = "pcbFoto";
            this.pcbFoto.Size = new System.Drawing.Size(292, 234);
            this.pcbFoto.TabIndex = 2;
            this.pcbFoto.TabStop = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(62)))), ((int)(((byte)(80)))));
            this.label4.Location = new System.Drawing.Point(651, 20);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 20);
            this.label4.TabIndex = 9;
            this.label4.Text = "Apellido:";
            // 
            // txtApellido
            // 
            this.txtApellido.BackColor = System.Drawing.Color.LightGray;
            this.txtApellido.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtApellido.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtApellido.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(62)))), ((int)(((byte)(80)))));
            this.txtApellido.Location = new System.Drawing.Point(655, 48);
            this.txtApellido.Margin = new System.Windows.Forms.Padding(4);
            this.txtApellido.Name = "txtApellido";
            this.txtApellido.ReadOnly = true;
            this.txtApellido.Size = new System.Drawing.Size(249, 25);
            this.txtApellido.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(62)))), ((int)(((byte)(80)))));
            this.label3.Location = new System.Drawing.Point(303, 20);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 20);
            this.label3.TabIndex = 7;
            this.label3.Text = "Nombre:";
            // 
            // txtNombre
            // 
            this.txtNombre.BackColor = System.Drawing.Color.LightGray;
            this.txtNombre.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNombre.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtNombre.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(62)))), ((int)(((byte)(80)))));
            this.txtNombre.Location = new System.Drawing.Point(307, 48);
            this.txtNombre.Margin = new System.Windows.Forms.Padding(4);
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.ReadOnly = true;
            this.txtNombre.Size = new System.Drawing.Size(256, 25);
            this.txtNombre.TabIndex = 6;
            // 
            // btnEscanearQR
            // 
            this.btnEscanearQR.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(196)))), ((int)(((byte)(15)))));
            this.btnEscanearQR.FlatAppearance.BorderSize = 0;
            this.btnEscanearQR.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(172)))), ((int)(((byte)(13)))));
            this.btnEscanearQR.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(185)))), ((int)(((byte)(14)))));
            this.btnEscanearQR.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEscanearQR.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnEscanearQR.ForeColor = System.Drawing.Color.White;
            this.btnEscanearQR.Location = new System.Drawing.Point(44, 20);
            this.btnEscanearQR.Margin = new System.Windows.Forms.Padding(4);
            this.btnEscanearQR.Name = "btnEscanearQR";
            this.btnEscanearQR.Size = new System.Drawing.Size(155, 35);
            this.btnEscanearQR.TabIndex = 5;
            this.btnEscanearQR.Text = "📱 Escanear QR";
            this.btnEscanearQR.UseVisualStyleBackColor = false;
            this.btnEscanearQR.Visible = false;
            this.btnEscanearQR.Click += new System.EventHandler(this.btnEscanearQR_Click);
            // 
            // dgvHistorialRegistroUpetino
            // 
            this.dgvHistorialRegistroUpetino.AllowUserToAddRows = false;
            this.dgvHistorialRegistroUpetino.AllowUserToDeleteRows = false;
            this.dgvHistorialRegistroUpetino.BackgroundColor = System.Drawing.Color.White;
            this.dgvHistorialRegistroUpetino.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvHistorialRegistroUpetino.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvHistorialRegistroUpetino.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(62)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(174)))), ((int)(((byte)(214)))), ((int)(((byte)(241)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(62)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvHistorialRegistroUpetino.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvHistorialRegistroUpetino.EnableHeadersVisualStyles = false;
            this.dgvHistorialRegistroUpetino.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(189)))), ((int)(((byte)(195)))), ((int)(((byte)(199)))));
            this.dgvHistorialRegistroUpetino.Location = new System.Drawing.Point(165, 688);
            this.dgvHistorialRegistroUpetino.Margin = new System.Windows.Forms.Padding(4);
            this.dgvHistorialRegistroUpetino.Name = "dgvHistorialRegistroUpetino";
            this.dgvHistorialRegistroUpetino.ReadOnly = true;
            this.dgvHistorialRegistroUpetino.RowHeadersVisible = false;
            this.dgvHistorialRegistroUpetino.RowHeadersWidth = 51;
            this.dgvHistorialRegistroUpetino.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvHistorialRegistroUpetino.Size = new System.Drawing.Size(1124, 195);
            this.dgvHistorialRegistroUpetino.TabIndex = 0;
            // 
            // frmRegistrarUpetino
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(62)))), ((int)(((byte)(80)))));
            this.ClientSize = new System.Drawing.Size(1444, 1055);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "frmRegistrarUpetino";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SCAE-UPT | Registro de Upetinos";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pcbFotoCapturada)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pcbCamara)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pcbFoto)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistorialRegistroUpetino)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnRetroceder;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtApellido;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.Button btnEscanearQR;
        private System.Windows.Forms.DataGridView dgvHistorialRegistroUpetino;
        private System.Windows.Forms.PictureBox pcbFoto;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pcbCamara;
        private System.Windows.Forms.Button btnApagarCamara;
        private System.Windows.Forms.Button btnCapturarFoto;
        private System.Windows.Forms.Button btnPrenderCamara;
        private System.Windows.Forms.Button btnCargarCamaras;
        private System.Windows.Forms.ComboBox cmbCamaras;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.PictureBox pcbFotoCapturada;
        private System.Windows.Forms.Label lblMensajeRostro;
        private System.Windows.Forms.Label label6;
    }
}