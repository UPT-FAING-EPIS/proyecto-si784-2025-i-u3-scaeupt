using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Management;
using SCAE_UPT.Entidad;
using SCAE_UPT.Negocio;
using SCAE_UPT.Presentacion;
using ZXing;


namespace SCAE_UPT
{
    public partial class frmLoginGuardiania : Form
    {
        public static String puertoSeleccionado;   
        public static String[] usuarioLogueado = new String[7];

        public frmLoginGuardiania()
        {
            InitializeComponent();
        }

        private void IniciarSesion(string user, string password)
        {
            
            clsNegocioUsuario objNegUsuario = new clsNegocioUsuario();
            DataTable usuarioDatos = objNegUsuario.MtdIniciarSesionOficina(user, password);

            foreach (DataRow row in usuarioDatos.Rows)
            {
                usuarioLogueado[0] = row[0].ToString(); //ID_USUARIO
                usuarioLogueado[1] = row[1].ToString(); //DNI
                usuarioLogueado[2] = row[2].ToString(); //NOMBRE
                usuarioLogueado[3] = row[3].ToString(); //Apellido
                usuarioLogueado[4] = row[4].ToString(); //Tipo
                usuarioLogueado[5] = row[5].ToString(); //Estado
                usuarioLogueado[6] = row[6].ToString(); //Usuario
            }
        }
        
        private void CambiarForm()
        {
            frmPrincipal frm = new frmPrincipal();
            frm.Show();
            this.Hide();
        }

        private void btnIniciarSesion_Click(object sender, EventArgs e)
        {
            string user = txtUsuario.Text;
            string password = txtContrasenia.Text;
            if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(password))
            {
                IniciarSesion(user,password);

                clsEntidadUsuarioRegistro objEntUsuarioRegistro = new clsEntidadUsuarioRegistro();
                clsNegocioUsuarioRegistro objNegUsuarioRegistro = new clsNegocioUsuarioRegistro();

                objEntUsuarioRegistro.ID_Empleado = usuarioLogueado[1];
                objEntUsuarioRegistro.FechaHora_Ingreso = DateTime.Now;

                objNegUsuarioRegistro.MtdGuardarIngreso(objEntUsuarioRegistro);

                if (usuarioLogueado.Length != 0)
                {
                    CambiarForm();
                }
            }
            
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            txtContrasenia.Text = string.Empty;
            txtUsuario.Text = string.Empty;
        }
    }
}
