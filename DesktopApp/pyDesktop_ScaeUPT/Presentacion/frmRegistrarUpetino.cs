using MySql.Data.MySqlClient;
using SCAE_UPT.Entidad;
using SCAE_UPT.Negocio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing;



using Newtonsoft.Json;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Net.Http;


namespace SCAE_UPT.Presentacion
{
    public partial class frmRegistrarUpetino : Form
    {


        Emgu.CV.VideoCapture captura;
        int camIndex = 0;
        Emgu.CV.Mat frame = new Emgu.CV.Mat();
        Emgu.CV.CascadeClassifier detectorRostro;
        Rectangle marco;
        byte[] fotoBytes, fotoCapturada;


        private readonly string _encryptionKey = "ScaeUPT2024SecretKey123456789012345";
        public frmRegistrarUpetino()
        {
            InitializeComponent();
            ListarRegistroUpetino();
            LimpiarTexto();
            cargarCuadroVerde();
            this.FormClosing += new FormClosingEventHandler(OnFormClosing);
        }

        public void cargarCuadroVerde()
        {
            //detectorRostro = new Emgu.CV.CascadeClassifier("haarcascade_frontalface_default.xml");
            string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Recursos", "haarcascade_frontalface_default.xml");
            detectorRostro = new Emgu.CV.CascadeClassifier(ruta);
            marco = new Rectangle(220, 100, 200, 260); // x, y, 

            captura = new Emgu.CV.VideoCapture(0);
            Application.Idle += MostrarFrame;
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult resultado = MessageBox.Show(
            "¿Estás seguro de que deseas salir?",
            "Confirmar salida",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question
            );

            if (resultado == DialogResult.Yes)
            {
                clsEntidadUsuarioRegistro objEntUsuarioRegistro = new clsEntidadUsuarioRegistro();
                clsNegocioUsuarioRegistro objNegUsuarioRegistro = new clsNegocioUsuarioRegistro();
                DateTime fechaHoraActual = DateTime.Now;

                objEntUsuarioRegistro.ID_Empleado = frmLoginGuardiania.usuarioLogueado[0];
                objEntUsuarioRegistro.FechaHora_Salida = fechaHoraActual;

                objNegUsuarioRegistro.MtdGuardarSalida(objEntUsuarioRegistro);

            }
            else
            {
                e.Cancel = true;
            }
        }

        private void ListarRegistroUpetino()
        {
            clsNegocioRegistro objNegRegistroVisitante = new clsNegocioRegistro();

            DataTable dataTable = objNegRegistroVisitante.ListaRegistro();

            dgvHistorialRegistroUpetino.DataSource = dataTable;
        }

        private void LimpiarTexto()
        {
            txtApellido.Text = string.Empty;
            txtNombre.Text = string.Empty;
            fotoCapturada = null;
            pcbFoto.Image = null;
            pcbFotoCapturada.Image = null;
        }

        private void btnRetroceder_Click(object sender, EventArgs e)
        {
            frmPrincipal frm = new frmPrincipal();
            frm.Show();
            this.Hide();
        }

        private async void RegistrarUpetino(string token)
        {
            clsNegocioRegistro objNegRegistro = new clsNegocioRegistro();

            try
            {
                if (string.IsNullOrWhiteSpace(token))
                    return;

                DataTable dtPersona = objNegRegistro.MtdBuscarPersonaPorDNI(token);

                if (dtPersona == null || dtPersona.Rows.Count == 0)
                {
                    MessageBox.Show("No se encontró ninguna persona con ese token.");
                    return;
                }

                // Solo debería haber una fila
                DataRow row = dtPersona.Rows[0];

                string dni = row["DNI"].ToString();
                string nombre = row["Nombre"].ToString();
                string apellido = row["Apellido"].ToString();
                string telefono = row["Telefono"].ToString();

                fotoBytes = row["Foto"] as byte[];

                txtNombre.Text = nombre;
                txtApellido.Text = apellido;
                MostrarFoto(fotoBytes, pcbFoto);

                bool rostroVerificado = await VerificarRostrosAsync(fotoCapturada, fotoBytes);
                if (rostroVerificado)
                {
                    ProcesarRegistro(dni, telefono);
                }
                else
                {
                    MessageBox.Show("No se puede procesar el registro. El rostro no coincide con la persona registrada.",
                                "Verificación fallida",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                    LimpiarTexto();
                }

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                Console.WriteLine("Error MySQL: " + ex.Message);
                MessageBox.Show("Error al registrar: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error general: " + ex.Message);
                MessageBox.Show("Error inesperado: " + ex.Message);
            }
        }


        private void ProcesarRegistro(string dni, string telefono)
        {
            clsNegocioRegistro objNegRegistro = new clsNegocioRegistro();

            DataTable dtRegistro = objNegRegistro.MtdBuscarRegistro(dni);

            if (dtRegistro.Rows.Count > 0)
            {
                DataRow registro = dtRegistro.Rows[0];
                var fechaSalida = registro["FechaHora_Salida"] as DateTime?;

                if (fechaSalida == null)
                    RegistrarSalida(dni);
                else
                    RegistrarIngreso(dni);
            }
            else
            {
                RegistrarIngreso(dni);
            }
        }

        private void RegistrarIngreso(string dni)
        {
            clsNegocioRegistro objNegRegistro = new clsNegocioRegistro();

            var entidad = new clsEntidadRegistro
            {
                DNI = dni,
                FechaHora_Ingreso = DateTime.Now,
                IDUsuario_Ingreso = frmLoginGuardiania.usuarioLogueado[0]
            };

            if (objNegRegistro.MtdGuardarIngresoUpetino(entidad))
            {
                objNegRegistro.MtdEliminarToken(dni);
                ListarRegistroUpetino();
                MessageBox.Show("Registro del usuario correcto!");
                LimpiarTexto();
            }
            else
            {
                MessageBox.Show("Error al registrar el ingreso.");
            }
        }

        private void RegistrarSalida(string dni)
        {
            clsNegocioRegistro objNegRegistro = new clsNegocioRegistro();

            var entidad = new clsEntidadRegistro
            {
                DNI = dni,
                FechaHora_Salida = DateTime.Now,
                IDUsuario_Salida = frmLoginGuardiania.usuarioLogueado[0]
            };

            if (objNegRegistro.MtdGuardarSalidaUpetino(entidad))
            {
                objNegRegistro.MtdEliminarToken(dni);
                ListarRegistroUpetino();
                MessageBox.Show("Salida registrada!");
                LimpiarTexto();
            }
            else
            {
                MessageBox.Show("Error al registrar la salida.");
            }
        }

        private void MostrarFoto(byte[] fotoBytes, PictureBox pcb)
        {
            if (fotoBytes == null || fotoBytes.Length == 0)
                return;

            using (var ms = new MemoryStream(fotoBytes))
            {
                try
                {
                    var image = Image.FromStream(ms);
                    var scaledImage = new Bitmap(image, new Size(307, 249));
                    pcb.SizeMode = PictureBoxSizeMode.StretchImage;
                    pcb.Image = scaledImage;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al mostrar la foto: " + ex.Message);
                }
            }
        }


        private string DecryptData(string encryptedData)
        {
            try
            {
                byte[] encryptedBytes = Convert.FromBase64String(encryptedData);
                string decryptedData;

                using (Aes aes = Aes.Create())
                {
                    // Derivar clave y vector de inicialización (IV) usando PBKDF2
                    using (var deriveBytes = new Rfc2898DeriveBytes(_encryptionKey, new byte[16], 1000))
                    {
                        aes.Key = deriveBytes.GetBytes(32); // 256 bits
                        aes.IV = deriveBytes.GetBytes(16);  // 128 bits
                    }

                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    // Crear desencriptador
                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    // Desencriptar los datos
                    using (var ms = new MemoryStream(encryptedBytes))
                    {
                        using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            using (var sr = new StreamReader(cs))
                            {
                                decryptedData = sr.ReadToEnd();
                            }
                        }
                    }
                }

                return decryptedData;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al desencriptar datos: " + ex);
                throw;
            }
        }

        private string EscanearQR()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Archivos de imagen|*.png;*.jpg;*.jpeg;*.bmp";
                openFileDialog.Title = "Seleccionar imagen con código QR";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Bitmap bitmap = (Bitmap)Bitmap.FromFile(openFileDialog.FileName);
                        BarcodeReader reader = new BarcodeReader();
                        var result = reader.Decode(bitmap);

                        if (result != null)
                        {
                            return result.Text;
                        }
                        else
                        {
                            MessageBox.Show("No se detectó ningún código QR en la imagen.", "QR no encontrado");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al leer la imagen: " + ex.Message, "Error");
                    }
                }
                return null;
            }
        }

        private void btnEscanearQR_Click(object sender, EventArgs e)
        {
            if (fotoCapturada == null)
            {
                MessageBox.Show("No se ha capturado la foto, por favor hagalo.", "Rostro Captura");
            }
            else
            {
                clsNegocioRegistro objNegRegistro = new clsNegocioRegistro();
                string tokenEncriptado = EscanearQR();

                if (tokenEncriptado == null)
                {
                    return;
                }

                string tokenDesencriptado = DecryptData(tokenEncriptado);

                string[] tokenSeparado = tokenDesencriptado.Split('|');

                if (ValidarHora(tokenSeparado[1]) == false)
                {
                    MessageBox.Show("QR Expirado");
                    objNegRegistro.MtdEliminarToken(tokenSeparado[0]);
                    return;
                }

                if (objNegRegistro.MtdCompararToken(tokenEncriptado, tokenSeparado[0]))
                {
                    RegistrarUpetino(tokenSeparado[0]);

                }
                else
                {
                    MessageBox.Show("No hay un token generado");
                }
            }
        }

        private void btnCargarCamaras_Click(object sender, EventArgs e)
        {
            CargarCamaras();
        }

        private void btnPrenderCamara_Click(object sender, EventArgs e)
        {
            if (cmbCamaras.SelectedIndex >= 0)
            {
                camIndex = cmbCamaras.SelectedIndex;
                captura = new Emgu.CV.VideoCapture(camIndex);
                Application.Idle += MostrarFrame;
            }
            else
            {
                MessageBox.Show("Tiene que seleccionar una cámara");
            }
        }

        private void btnCapturarFoto_Click(object sender, EventArgs e)
        {
            fotoCapturada = CapturarImagen();
            MostrarFoto(fotoCapturada, pcbFotoCapturada);
        }

        private void btnApagarCamara_Click(object sender, EventArgs e)
        {
            if (captura != null)
            {
                Application.Idle -= MostrarFrame;
                captura.Dispose();
                pcbCamara.Image = null;
            }
        }


        private void CargarCamaras()
        {
            cmbCamaras.Items.Clear();
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    var cap = new Emgu.CV.VideoCapture(i);
                    if (cap.IsOpened)
                    {
                        cmbCamaras.Items.Add($"Cámara {i}");
                        cap.Dispose();
                    }
                }
                catch { }
            }

            if (cmbCamaras.Items.Count > 0)
            {
                cmbCamaras.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("No se encontraron cámaras.");
            }
        }

        private void MostrarFrame(object sender, EventArgs e)
        {
            captura.Read(frame);
            if (frame.IsEmpty) return;

            Emgu.CV.Mat frameConGuia = frame.Clone();

            // escala de grises para detectar rostro
            using (var gray = frame.ToImage<Gray, byte>())
            {
                Rectangle[] rostros = detectorRostro.DetectMultiScale(gray, 1.1, 4);

                // marco verde
                CvInvoke.Rectangle(frameConGuia, marco, new MCvScalar(0, 255, 0), 2);

                bool rostroDentro = false;

                foreach (var r in rostros)
                {
                    CvInvoke.Rectangle(frameConGuia, r, new MCvScalar(255, 0, 0), 2); // dibuja el rostro detectado

                    if (marco.Contains(r.Location) && marco.Contains(new System.Drawing.Point(r.Right, r.Bottom)))
                    {
                        rostroDentro = true;
                        break;
                    }
                }

                lblMensajeRostro.Text = rostroDentro ? "✅ Rostro alineado correctamente" : "❗Alinea tu rostro dentro del marco verde";
            }

            pcbCamara.Image?.Dispose();
            pcbCamara.Image = frameConGuia.ToImage<Bgr, byte>().ToBitmap();

        }

        public byte[] CapturarImagen()
        {
            if (frame.IsEmpty)
            {
                MessageBox.Show("No hay imagen para capturar.");
                return null;
            }

            Bitmap bmp = frame.ToImage<Bgr, byte>().ToBitmap();
            using (MemoryStream ms = new MemoryStream())
            {
                bmp.Save(ms, ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        private async Task<bool> VerificarRostrosAsync(byte[] foto1, byte[] foto2)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var payload = new
                    {
                        img1 = Convert.ToBase64String(foto1),
                        img2 = Convert.ToBase64String(foto2)
                    };

                    var content = new StringContent(
                        JsonConvert.SerializeObject(payload),
                        Encoding.UTF8,
                        "application/json"
                    );

                    HttpResponseMessage response = await client.PostAsync("https://scae-upt-python-service.azurewebsites.net/verificar", content);
                    response.EnsureSuccessStatusCode();

                    string json = await response.Content.ReadAsStringAsync();
                    dynamic resultado = JsonConvert.DeserializeObject(json);

                    string mensaje = resultado.resultado;
                    bool rostroCoincide=false;

                    if (mensaje == "VERIFICACION EXITOSA")
                    {
                        MessageBox.Show(
                            "✅ Rostro verificado con éxito",
                            "Resultado",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                        rostroCoincide = true;
                    }
                    else if (mensaje == "COINCIDE PERO IMAGEN SOSPECHOSA")
                    {
                        MessageBox.Show(
                            "⚠️ Rostro coincide pero sospechosamente",
                            "Resultado",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                        rostroCoincide = false;
                    }
                    else
                    {
                        MessageBox.Show("❌ Error al verificar rostro: " + mensaje, "Resultado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        rostroCoincide = false;
                    }


                    return rostroCoincide;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al verificar rostro: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }



        public bool ValidarHora(string horaParametro)
        {
            try
            {
                DateTime horaRecibida = DateTime.ParseExact(horaParametro, "yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
                DateTime horaActual = DateTime.Now;

                TimeSpan diferencia = horaActual - horaRecibida;
                double minutos = Math.Abs(diferencia.TotalMinutes);

                return minutos <= 5;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al comparar horas: " + ex.Message);
                return false;
            }
        }
    }
}
