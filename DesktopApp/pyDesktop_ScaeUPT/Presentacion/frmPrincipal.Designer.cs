namespace SCAE_UPT.Presentacion
{
    partial class frmPrincipal
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
            this.panelHeader = new System.Windows.Forms.Panel();
            this.lblSubtitulo = new System.Windows.Forms.Label();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.panelMain = new System.Windows.Forms.Panel();
            this.panelHistoriales = new System.Windows.Forms.Panel();
            this.btnHistorialRegistroUpetinos = new System.Windows.Forms.Button();
            this.btnHistorialRegistroVisitantes = new System.Windows.Forms.Button();
            this.lblHistoriales = new System.Windows.Forms.Label();
            this.panelRegistros = new System.Windows.Forms.Panel();
            this.btnRegistroUpetinos = new System.Windows.Forms.Button();
            this.btnRegistroVisitantes = new System.Windows.Forms.Button();
            this.lblRegistros = new System.Windows.Forms.Label();
            this.panelFooter = new System.Windows.Forms.Panel();
            this.lblFooter = new System.Windows.Forms.Label();
            this.panelHeader.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.panelHistoriales.SuspendLayout();
            this.panelRegistros.SuspendLayout();
            this.panelFooter.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.panelHeader.Controls.Add(this.lblSubtitulo);
            this.panelHeader.Controls.Add(this.lblTitulo);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(1000, 120);
            this.panelHeader.TabIndex = 0;
            // 
            // lblSubtitulo
            // 
            this.lblSubtitulo.AutoSize = true;
            this.lblSubtitulo.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblSubtitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(189)))), ((int)(((byte)(195)))), ((int)(((byte)(199)))));
            this.lblSubtitulo.Location = new System.Drawing.Point(35, 75);
            this.lblSubtitulo.Name = "lblSubtitulo";
            this.lblSubtitulo.Size = new System.Drawing.Size(392, 21);
            this.lblSubtitulo.TabIndex = 1;
            this.lblSubtitulo.Text = "Sistema de Control de Acceso Electronico - Universidad";
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.White;
            this.lblTitulo.Location = new System.Drawing.Point(30, 25);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(372, 45);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "🏛️ SISTEMA SCAE-UPT";
            // 
            // panelMain
            // 
            this.panelMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(240)))), ((int)(((byte)(241)))));
            this.panelMain.Controls.Add(this.panelHistoriales);
            this.panelMain.Controls.Add(this.panelRegistros);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 120);
            this.panelMain.Name = "panelMain";
            this.panelMain.Padding = new System.Windows.Forms.Padding(30);
            this.panelMain.Size = new System.Drawing.Size(1000, 480);
            this.panelMain.TabIndex = 1;
            // 
            // panelHistoriales
            // 
            this.panelHistoriales.BackColor = System.Drawing.Color.White;
            this.panelHistoriales.Controls.Add(this.btnHistorialRegistroUpetinos);
            this.panelHistoriales.Controls.Add(this.btnHistorialRegistroVisitantes);
            this.panelHistoriales.Controls.Add(this.lblHistoriales);
            this.panelHistoriales.Location = new System.Drawing.Point(511, 47);
            this.panelHistoriales.Name = "panelHistoriales";
            this.panelHistoriales.Size = new System.Drawing.Size(280, 380);
            this.panelHistoriales.TabIndex = 1;
            // 
            // btnHistorialRegistroUpetinos
            // 
            this.btnHistorialRegistroUpetinos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(196)))), ((int)(((byte)(15)))));
            this.btnHistorialRegistroUpetinos.FlatAppearance.BorderSize = 0;
            this.btnHistorialRegistroUpetinos.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(172)))), ((int)(((byte)(13)))));
            this.btnHistorialRegistroUpetinos.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(208)))), ((int)(((byte)(63)))));
            this.btnHistorialRegistroUpetinos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHistorialRegistroUpetinos.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnHistorialRegistroUpetinos.ForeColor = System.Drawing.Color.White;
            this.btnHistorialRegistroUpetinos.Image = global::SCAE_UPT.Properties.Resources.calendar_empty;
            this.btnHistorialRegistroUpetinos.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnHistorialRegistroUpetinos.Location = new System.Drawing.Point(20, 222);
            this.btnHistorialRegistroUpetinos.Name = "btnHistorialRegistroUpetinos";
            this.btnHistorialRegistroUpetinos.Size = new System.Drawing.Size(240, 100);
            this.btnHistorialRegistroUpetinos.TabIndex = 2;
            this.btnHistorialRegistroUpetinos.Text = "📈 Historial\nUpetinos";
            this.btnHistorialRegistroUpetinos.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnHistorialRegistroUpetinos.UseVisualStyleBackColor = false;
            this.btnHistorialRegistroUpetinos.Click += new System.EventHandler(this.btnHistorialRegistroUpetinos_Click);
            // 
            // btnHistorialRegistroVisitantes
            // 
            this.btnHistorialRegistroVisitantes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(196)))), ((int)(((byte)(15)))));
            this.btnHistorialRegistroVisitantes.FlatAppearance.BorderSize = 0;
            this.btnHistorialRegistroVisitantes.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(172)))), ((int)(((byte)(13)))));
            this.btnHistorialRegistroVisitantes.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(208)))), ((int)(((byte)(63)))));
            this.btnHistorialRegistroVisitantes.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHistorialRegistroVisitantes.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnHistorialRegistroVisitantes.ForeColor = System.Drawing.Color.White;
            this.btnHistorialRegistroVisitantes.Image = global::SCAE_UPT.Properties.Resources.calendar_empty;
            this.btnHistorialRegistroVisitantes.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnHistorialRegistroVisitantes.Location = new System.Drawing.Point(20, 90);
            this.btnHistorialRegistroVisitantes.Name = "btnHistorialRegistroVisitantes";
            this.btnHistorialRegistroVisitantes.Size = new System.Drawing.Size(240, 100);
            this.btnHistorialRegistroVisitantes.TabIndex = 1;
            this.btnHistorialRegistroVisitantes.Text = "📋 Historial\nVisitantes";
            this.btnHistorialRegistroVisitantes.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnHistorialRegistroVisitantes.UseVisualStyleBackColor = false;
            this.btnHistorialRegistroVisitantes.Click += new System.EventHandler(this.btnHistorialRegistroVisitantes_Click);
            // 
            // lblHistoriales
            // 
            this.lblHistoriales.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(126)))), ((int)(((byte)(34)))));
            this.lblHistoriales.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblHistoriales.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblHistoriales.ForeColor = System.Drawing.Color.White;
            this.lblHistoriales.Location = new System.Drawing.Point(0, 0);
            this.lblHistoriales.Name = "lblHistoriales";
            this.lblHistoriales.Size = new System.Drawing.Size(280, 60);
            this.lblHistoriales.TabIndex = 0;
            this.lblHistoriales.Text = "📊 HISTORIALES";
            this.lblHistoriales.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelRegistros
            // 
            this.panelRegistros.BackColor = System.Drawing.Color.White;
            this.panelRegistros.Controls.Add(this.btnRegistroUpetinos);
            this.panelRegistros.Controls.Add(this.btnRegistroVisitantes);
            this.panelRegistros.Controls.Add(this.lblRegistros);
            this.panelRegistros.Location = new System.Drawing.Point(109, 47);
            this.panelRegistros.Name = "panelRegistros";
            this.panelRegistros.Size = new System.Drawing.Size(280, 380);
            this.panelRegistros.TabIndex = 0;
            // 
            // btnRegistroUpetinos
            // 
            this.btnRegistroUpetinos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.btnRegistroUpetinos.FlatAppearance.BorderSize = 0;
            this.btnRegistroUpetinos.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.btnRegistroUpetinos.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(144)))), ((int)(((byte)(226)))));
            this.btnRegistroUpetinos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRegistroUpetinos.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnRegistroUpetinos.ForeColor = System.Drawing.Color.White;
            this.btnRegistroUpetinos.Image = global::SCAE_UPT.Properties.Resources.community_users;
            this.btnRegistroUpetinos.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnRegistroUpetinos.Location = new System.Drawing.Point(20, 222);
            this.btnRegistroUpetinos.Name = "btnRegistroUpetinos";
            this.btnRegistroUpetinos.Size = new System.Drawing.Size(240, 100);
            this.btnRegistroUpetinos.TabIndex = 2;
            this.btnRegistroUpetinos.Text = "🎓 Registro\nUpetinos";
            this.btnRegistroUpetinos.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnRegistroUpetinos.UseVisualStyleBackColor = false;
            this.btnRegistroUpetinos.Click += new System.EventHandler(this.btnRegistroUpetinos_Click);
            // 
            // btnRegistroVisitantes
            // 
            this.btnRegistroVisitantes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.btnRegistroVisitantes.FlatAppearance.BorderSize = 0;
            this.btnRegistroVisitantes.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.btnRegistroVisitantes.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(144)))), ((int)(((byte)(226)))));
            this.btnRegistroVisitantes.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRegistroVisitantes.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnRegistroVisitantes.ForeColor = System.Drawing.Color.White;
            this.btnRegistroVisitantes.Image = global::SCAE_UPT.Properties.Resources.note_edit;
            this.btnRegistroVisitantes.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnRegistroVisitantes.Location = new System.Drawing.Point(20, 90);
            this.btnRegistroVisitantes.Name = "btnRegistroVisitantes";
            this.btnRegistroVisitantes.Size = new System.Drawing.Size(240, 100);
            this.btnRegistroVisitantes.TabIndex = 1;
            this.btnRegistroVisitantes.Text = "👥 Registro\nVisitantes";
            this.btnRegistroVisitantes.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnRegistroVisitantes.UseVisualStyleBackColor = false;
            this.btnRegistroVisitantes.Click += new System.EventHandler(this.btnRegistroVisitantes_Click);
            // 
            // lblRegistros
            // 
            this.lblRegistros.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.lblRegistros.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblRegistros.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblRegistros.ForeColor = System.Drawing.Color.White;
            this.lblRegistros.Location = new System.Drawing.Point(0, 0);
            this.lblRegistros.Name = "lblRegistros";
            this.lblRegistros.Size = new System.Drawing.Size(280, 60);
            this.lblRegistros.TabIndex = 0;
            this.lblRegistros.Text = "📝 REGISTROS";
            this.lblRegistros.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelFooter
            // 
            this.panelFooter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(62)))), ((int)(((byte)(80)))));
            this.panelFooter.Controls.Add(this.lblFooter);
            this.panelFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelFooter.Location = new System.Drawing.Point(0, 600);
            this.panelFooter.Name = "panelFooter";
            this.panelFooter.Size = new System.Drawing.Size(1000, 50);
            this.panelFooter.TabIndex = 2;
            // 
            // lblFooter
            // 
            this.lblFooter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblFooter.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblFooter.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(189)))), ((int)(((byte)(195)))), ((int)(((byte)(199)))));
            this.lblFooter.Location = new System.Drawing.Point(0, 0);
            this.lblFooter.Name = "lblFooter";
            this.lblFooter.Size = new System.Drawing.Size(1000, 50);
            this.lblFooter.TabIndex = 0;
            this.lblFooter.Text = "© 2025 Universidad Privada de Tacna - Sistema SCAE-UPT";
            this.lblFooter.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblFooter.Click += new System.EventHandler(this.lblFooter_Click);
            // 
            // frmPrincipal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(240)))), ((int)(((byte)(241)))));
            this.ClientSize = new System.Drawing.Size(1000, 650);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelHeader);
            this.Controls.Add(this.panelFooter);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "frmPrincipal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "🏛️ SCAE-UPT - Sistema Principal";
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.panelMain.ResumeLayout(false);
            this.panelHistoriales.ResumeLayout(false);
            this.panelRegistros.ResumeLayout(false);
            this.panelFooter.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblSubtitulo;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Panel panelRegistros;
        private System.Windows.Forms.Label lblRegistros;
        private System.Windows.Forms.Button btnRegistroVisitantes;
        private System.Windows.Forms.Button btnRegistroUpetinos;
        private System.Windows.Forms.Panel panelHistoriales;
        private System.Windows.Forms.Label lblHistoriales;
        private System.Windows.Forms.Button btnHistorialRegistroVisitantes;
        private System.Windows.Forms.Button btnHistorialRegistroUpetinos;
        private System.Windows.Forms.Panel panelFooter;
        private System.Windows.Forms.Label lblFooter;
    }
}