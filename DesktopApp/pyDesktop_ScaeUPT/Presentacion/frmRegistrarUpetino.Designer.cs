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
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.btnRetroceder);
            this.panel1.Location = new System.Drawing.Point(16, 15);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1449, 80);
            this.panel1.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(557, 20);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(416, 46);
            this.label1.TabIndex = 1;
            this.label1.Text = "REGISTRO UPETINO";
            // 
            // btnRetroceder
            // 
            this.btnRetroceder.Location = new System.Drawing.Point(44, 37);
            this.btnRetroceder.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnRetroceder.Name = "btnRetroceder";
            this.btnRetroceder.Size = new System.Drawing.Size(100, 28);
            this.btnRetroceder.TabIndex = 0;
            this.btnRetroceder.Text = "Retroceder";
            this.btnRetroceder.UseVisualStyleBackColor = true;
            this.btnRetroceder.Click += new System.EventHandler(this.btnRetroceder_Click);
            // 
            // panel2
            // 
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
            this.panel2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1578, 1076);
            this.panel2.TabIndex = 7;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(161, 719);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(107, 16);
            this.label6.TabIndex = 20;
            this.label6.Text = "Ultimos registros";
            // 
            // lblMensajeRostro
            // 
            this.lblMensajeRostro.AutoSize = true;
            this.lblMensajeRostro.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMensajeRostro.Location = new System.Drawing.Point(496, 629);
            this.lblMensajeRostro.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMensajeRostro.Name = "lblMensajeRostro";
            this.lblMensajeRostro.Size = new System.Drawing.Size(411, 31);
            this.lblMensajeRostro.TabIndex = 19;
            this.lblMensajeRostro.Text = "Mensaje sobre Alineacion Rostro";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(1249, 295);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(100, 16);
            this.label5.TabIndex = 18;
            this.label5.Text = "Foto Capturada";
            // 
            // pcbFotoCapturada
            // 
            this.pcbFotoCapturada.Location = new System.Drawing.Point(1265, 315);
            this.pcbFotoCapturada.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pcbFotoCapturada.Name = "pcbFotoCapturada";
            this.pcbFotoCapturada.Size = new System.Drawing.Size(292, 234);
            this.pcbFotoCapturada.TabIndex = 17;
            this.pcbFotoCapturada.TabStop = false;
            // 
            // btnApagarCamara
            // 
            this.btnApagarCamara.Location = new System.Drawing.Point(44, 358);
            this.btnApagarCamara.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnApagarCamara.Name = "btnApagarCamara";
            this.btnApagarCamara.Size = new System.Drawing.Size(155, 28);
            this.btnApagarCamara.TabIndex = 16;
            this.btnApagarCamara.Text = "Apagar Camara";
            this.btnApagarCamara.UseVisualStyleBackColor = true;
            this.btnApagarCamara.Click += new System.EventHandler(this.btnApagarCamara_Click);
            // 
            // btnCapturarFoto
            // 
            this.btnCapturarFoto.Location = new System.Drawing.Point(44, 299);
            this.btnCapturarFoto.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCapturarFoto.Name = "btnCapturarFoto";
            this.btnCapturarFoto.Size = new System.Drawing.Size(155, 28);
            this.btnCapturarFoto.TabIndex = 15;
            this.btnCapturarFoto.Text = "Capturar";
            this.btnCapturarFoto.UseVisualStyleBackColor = true;
            this.btnCapturarFoto.Click += new System.EventHandler(this.btnCapturarFoto_Click);
            // 
            // btnPrenderCamara
            // 
            this.btnPrenderCamara.Location = new System.Drawing.Point(44, 245);
            this.btnPrenderCamara.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnPrenderCamara.Name = "btnPrenderCamara";
            this.btnPrenderCamara.Size = new System.Drawing.Size(156, 28);
            this.btnPrenderCamara.TabIndex = 14;
            this.btnPrenderCamara.Text = "Prender Camara";
            this.btnPrenderCamara.UseVisualStyleBackColor = true;
            this.btnPrenderCamara.Click += new System.EventHandler(this.btnPrenderCamara_Click);
            // 
            // btnCargarCamaras
            // 
            this.btnCargarCamaras.Location = new System.Drawing.Point(44, 91);
            this.btnCargarCamaras.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCargarCamaras.Name = "btnCargarCamaras";
            this.btnCargarCamaras.Size = new System.Drawing.Size(156, 28);
            this.btnCargarCamaras.TabIndex = 13;
            this.btnCargarCamaras.Text = "Cargar Camaras";
            this.btnCargarCamaras.UseVisualStyleBackColor = true;
            this.btnCargarCamaras.Click += new System.EventHandler(this.btnCargarCamaras_Click);
            // 
            // cmbCamaras
            // 
            this.cmbCamaras.FormattingEnabled = true;
            this.cmbCamaras.Location = new System.Drawing.Point(44, 127);
            this.cmbCamaras.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbCamaras.Name = "cmbCamaras";
            this.cmbCamaras.Size = new System.Drawing.Size(153, 24);
            this.cmbCamaras.TabIndex = 12;
            // 
            // pcbCamara
            // 
            this.pcbCamara.Location = new System.Drawing.Point(303, 91);
            this.pcbCamara.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pcbCamara.Name = "pcbCamara";
            this.pcbCamara.Size = new System.Drawing.Size(938, 534);
            this.pcbCamara.TabIndex = 11;
            this.pcbCamara.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1249, 20);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 16);
            this.label2.TabIndex = 10;
            this.label2.Text = "Foto";
            // 
            // pcbFoto
            // 
            this.pcbFoto.Location = new System.Drawing.Point(1265, 39);
            this.pcbFoto.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pcbFoto.Name = "pcbFoto";
            this.pcbFoto.Size = new System.Drawing.Size(292, 234);
            this.pcbFoto.TabIndex = 2;
            this.pcbFoto.TabStop = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(651, 20);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 16);
            this.label4.TabIndex = 9;
            this.label4.Text = "Apellido:";
            // 
            // txtApellido
            // 
            this.txtApellido.Location = new System.Drawing.Point(655, 39);
            this.txtApellido.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtApellido.Name = "txtApellido";
            this.txtApellido.ReadOnly = true;
            this.txtApellido.Size = new System.Drawing.Size(249, 22);
            this.txtApellido.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(303, 20);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 16);
            this.label3.TabIndex = 7;
            this.label3.Text = "Nombre:";
            // 
            // txtNombre
            // 
            this.txtNombre.Location = new System.Drawing.Point(307, 39);
            this.txtNombre.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.ReadOnly = true;
            this.txtNombre.Size = new System.Drawing.Size(256, 22);
            this.txtNombre.TabIndex = 6;
            // 
            // btnEscanearQR
            // 
            this.btnEscanearQR.Location = new System.Drawing.Point(44, 20);
            this.btnEscanearQR.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnEscanearQR.Name = "btnEscanearQR";
            this.btnEscanearQR.Size = new System.Drawing.Size(155, 28);
            this.btnEscanearQR.TabIndex = 5;
            this.btnEscanearQR.Text = "Escanear QR";
            this.btnEscanearQR.UseVisualStyleBackColor = true;
            this.btnEscanearQR.Visible = false;
            this.btnEscanearQR.Click += new System.EventHandler(this.btnEscanearQR_Click);
            // 
            // dgvHistorialRegistroUpetino
            // 
            this.dgvHistorialRegistroUpetino.AllowUserToAddRows = false;
            this.dgvHistorialRegistroUpetino.AllowUserToDeleteRows = false;
            this.dgvHistorialRegistroUpetino.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvHistorialRegistroUpetino.Location = new System.Drawing.Point(221, 746);
            this.dgvHistorialRegistroUpetino.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dgvHistorialRegistroUpetino.Name = "dgvHistorialRegistroUpetino";
            this.dgvHistorialRegistroUpetino.ReadOnly = true;
            this.dgvHistorialRegistroUpetino.RowHeadersWidth = 51;
            this.dgvHistorialRegistroUpetino.Size = new System.Drawing.Size(1124, 304);
            this.dgvHistorialRegistroUpetino.TabIndex = 0;
            // 
            // frmRegistrarUpetino
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1607, 1055);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "frmRegistrarUpetino";
            this.Text = "frmRegistrarUpetinos";
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