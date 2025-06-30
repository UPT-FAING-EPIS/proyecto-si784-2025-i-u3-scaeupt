using Org.BouncyCastle.Pqc.Crypto.Frodo;
using SCAE_UPT.Entidad;
using SCAE_UPT.Negocio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms; 

namespace SCAE_UPT.Presentacion
{
    public partial class frmPrincipal : Form
    {
        public frmPrincipal()
        {
            InitializeComponent();
            ConfigurarEstilosAdicionales();

        }

        private void ConfigurarEstilosAdicionales()
        {
            // Configurar sombras y efectos para los paneles
            ConfigurarSombraPaneles();
            
            // Configurar tooltips informativos
            ConfigurarTooltips();
            
            // Configurar efectos hover adicionales
            ConfigurarEfectosHover();
        }

        private void ConfigurarSombraPaneles()
        {
            // Agregar bordes redondeados simulados con padding
            panelRegistros.Padding = new Padding(5);
            panelHistoriales.Padding = new Padding(5);
        }

        private void ConfigurarTooltips()
        {
            ToolTip toolTip = new ToolTip();
            toolTip.SetToolTip(btnRegistroVisitantes, "Registrar entrada y salida de visitantes");
            toolTip.SetToolTip(btnRegistroUpetinos, "Registrar entrada y salida de estudiantes UPT");
            toolTip.SetToolTip(btnHistorialRegistroVisitantes, "Ver historial completo de visitantes");
            toolTip.SetToolTip(btnHistorialRegistroUpetinos, "Ver historial completo de estudiantes");
        }

        private void ConfigurarEfectosHover()
        {
            // Los efectos hover ya están configurados en el Designer
            // Aquí se pueden agregar efectos adicionales si es necesario
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult resultado = MessageBox.Show(
            "¿Estás seguro de que deseas salir?",
            "Confirmar salida",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question
            );

            if(resultado == DialogResult.Yes)
            {
                

            }
            else
            {
                e.Cancel = true;
            }
        }

        private void btnRegistroVisitantes_Click(object sender, EventArgs e)
        {
            frmRegistroVisitante frm  = new frmRegistroVisitante();
            frm.Show();
            this.Hide();
        }

        private void btnHistorialRegistroVisitantes_Click(object sender, EventArgs e)
        {
            frmHistorialVisitante frm = new frmHistorialVisitante();
            frm.Show();
            this.Hide();
        }

        private void btnHistorialRegistroUpetinos_Click(object sender, EventArgs e)
        {
            frmHistorialUpetino frm = new frmHistorialUpetino();
            frm.Show();
            this.Hide();
        }

        private void btnRegistroUpetinos_Click(object sender, EventArgs e)
        {
            frmRegistrarUpetino frm = new frmRegistrarUpetino();
            frm.Show();
            this.Hide();
        }

        private void btnAdministrarUsuarios_Click(object sender, EventArgs e)
        {
            frmAdministrarUsuario frm = new frmAdministrarUsuario();
            frm.Show();
            this.Hide();
        }

        private void btnAdministrarUpetinos_Click(object sender, EventArgs e)
        {
            frmAdministrarUpetino frm = new frmAdministrarUpetino();
            frm.Show();
            this.Hide();
        }

        private int IdentificarUsuario()
        {
            if ("Administrador".Equals(frmLoginGuardiania.usuarioLogueado[4]))
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        private void lblFooter_Click(object sender, EventArgs e)
        {

        }
    }
}
