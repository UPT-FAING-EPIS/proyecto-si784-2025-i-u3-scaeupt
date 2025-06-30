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



        private BarcodeReader qrReader;
        private bool procesamientoEnCurso = false;
        private System.Windows.Forms.Timer timerDeteccion;

        private Rectangle marcoQR;

        private readonly string _encryptionKey = "ScaeUPT2024SecretKey123456789012345";
        public frmRegistrarUpetino()
        {
            InitializeComponent();
            ConfigurarEstilosAdicionales();
            ListarRegistroUpetino();
            LimpiarTexto();
            cargarCuadroVerde();
            InicializarDeteccionAutomatica(); // Nueva función
            this.FormClosing += new FormClosingEventHandler(OnFormClosing);
        }

        
        private void ConfigurarEstilosAdicionales()
        {
            // Configurar bordes redondeados para los PictureBox
            pcbCamara.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, pcbCamara.ClientRectangle,
                    Color.FromArgb(52, 152, 219), 2, ButtonBorderStyle.Solid,
                    Color.FromArgb(52, 152, 219), 2, ButtonBorderStyle.Solid,
                    Color.FromArgb(52, 152, 219), 2, ButtonBorderStyle.Solid,
                    Color.FromArgb(52, 152, 219), 2, ButtonBorderStyle.Solid);
            };
    
            // Configurar el DataGridView con estilos adicionales
            dgvHistorialRegistroUpetino.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            //dgvHistorialRegistroUpetino.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvHistorialRegistroUpetino.DefaultCellStyle.Padding = new Padding(1);
    
            // Configurar tooltips informativos
            ToolTip toolTip = new ToolTip();
            toolTip.SetToolTip(btnCargarCamaras, "Detecta y carga las cámaras disponibles");
            toolTip.SetToolTip(btnPrenderCamara, "Inicia la cámara seleccionada");
            toolTip.SetToolTip(btnCapturarFoto, "Captura una foto del rostro detectado");
            toolTip.SetToolTip(btnApagarCamara, "Detiene la cámara activa");
        }



        private void InicializarDeteccionAutomatica()
        {
            qrReader = new BarcodeReader
            {
                AutoRotate = true,
                TryInverted = true,
                Options = new ZXing.Common.DecodingOptions
                {
                    TryHarder = true,
                    PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.QR_CODE },
                    // Agregar más opciones para mejorar detección
                    ReturnCodabarStartEnd = false,
                    PureBarcode = false
                }
            };

            // Timer más frecuente para mejor detección
            timerDeteccion = new System.Windows.Forms.Timer();
            timerDeteccion.Interval = 300; // Cada 300ms en lugar de 500ms
            timerDeteccion.Tick += TimerDeteccion_Tick;
            timerDeteccion.Start();
        }


        private void TimerDeteccion_Tick(object sender, EventArgs e)
        {
            if (!procesamientoEnCurso && frame != null && !frame.IsEmpty)
            {
                DetectarQRAutomatico();
            }
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

        private async Task RegistrarUpetino(string token)
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

                bool rostroVerificado = await VerificarRostrosAsync(fotoBytes, fotoCapturada);
                if (rostroVerificado)
                {
                    ProcesarRegistro(dni, telefono);
                }
                else
                {
                    MessageBox.Show("No se puede procesar el registro.",
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

        // private void btnEscanearQR_Click(object sender, EventArgs e)
        // {
        //     if (fotoCapturada == null)
        //     {
        //         MessageBox.Show("No se ha capturado la foto, por favor hagalo.", "Rostro Captura");
        //     }
        //     else
        //     {
        //         clsNegocioRegistro objNegRegistro = new clsNegocioRegistro();
        //         string tokenEncriptado = EscanearQR();

        //         if (tokenEncriptado == null)
        //         {
        //             return;
        //         }

        //         string tokenDesencriptado = DecryptData(tokenEncriptado);

        //         string[] tokenSeparado = tokenDesencriptado.Split('|');

        //         if (ValidarHora(tokenSeparado[1]) == false)
        //         {
        //             MessageBox.Show("QR Expirado");
        //             objNegRegistro.MtdEliminarToken(tokenSeparado[0]);
        //             return;
        //         }

        //         if (objNegRegistro.MtdCompararToken(tokenEncriptado, tokenSeparado[0]))
        //         {
        //             RegistrarUpetino(tokenSeparado[0]);

        //         }
        //         else
        //         {
        //             MessageBox.Show("El codigo QR ya fue usado");
        //         }
        //     }
        // }

        private void btnEscanearQR_Click(object sender, EventArgs e)
        {
            MessageBox.Show("El escaneo ahora es automático. Solo muestra tu QR frente a la cámara.", "Modo Automático");
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
            if (ValidarRostroCentrado())
            {
                fotoCapturada = CapturarRostroRecortado();
                MostrarFoto(fotoCapturada, pcbFotoCapturada);
                MessageBox.Show("Rostro capturado y recortado correctamente!", "Captura Exitosa");
            }
            else
            {
                MessageBox.Show("Por favor, centra tu rostro dentro del marco verde antes de capturar.", "Rostro no centrado");
            }
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

        public byte[] CapturarRostroRecortado()
        {
            if (frame.IsEmpty)
            {
                MessageBox.Show("No hay imagen para capturar.");
                return null;
            }

            try
            {
                using (var gray = frame.ToImage<Gray, byte>())
                {
                    Rectangle[] rostros = detectorRostro.DetectMultiScale(gray, 1.1, 4);

                    foreach (var rostro in rostros)
                    {
                        // Verificar si el rostro está completamente dentro del marco verde
                        if (marco.Contains(rostro.Location) &&
                            marco.Contains(new System.Drawing.Point(rostro.Right, rostro.Bottom)))
                        {
                            // Expandir ligeramente el área del rostro para mejor captura
                            int margen = 20;
                            Rectangle rostroExpandido = new Rectangle(
                                Math.Max(0, rostro.X - margen),
                                Math.Max(0, rostro.Y - margen),
                                Math.Min(frame.Width - Math.Max(0, rostro.X - margen), rostro.Width + (margen * 2)),
                                Math.Min(frame.Height - Math.Max(0, rostro.Y - margen), rostro.Height + (margen * 2))
                            );

                            // Convertir frame a bitmap y recortar el rostro
                            Bitmap frameBitmap = frame.ToImage<Bgr, byte>().ToBitmap();
                            Bitmap rostroRecortado = frameBitmap.Clone(rostroExpandido, frameBitmap.PixelFormat);

                            // Convertir a bytes
                            using (MemoryStream ms = new MemoryStream())
                            {
                                rostroRecortado.Save(ms, ImageFormat.Jpeg);
                                byte[] rostroBytes = ms.ToArray();

                                // Limpiar recursos
                                frameBitmap.Dispose();
                                rostroRecortado.Dispose();

                                return rostroBytes;
                            }
                        }
                    }
                }

                // Si no se encuentra rostro centrado, capturar imagen completa como respaldo
                return CapturarImagen();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al capturar rostro recortado: " + ex.Message);
                // En caso de error, usar captura normal
                return CapturarImagen();
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
                        img1 = "data:image/jpeg;base64," + Convert.ToBase64String(foto1),
                        img2 = "data:image/jpeg;base64," + Convert.ToBase64String(foto2)
                    };

                    var content = new StringContent(
                        JsonConvert.SerializeObject(payload),
                        Encoding.UTF8,
                        "application/json"
                    );

                    HttpResponseMessage response = await client.PostAsync("http://127.0.0.1:5000/verificar", content);

                    if (!response.IsSuccessStatusCode)
                    {
                        string errorJson = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"❌ Error al verificar rostro:\nCódigo: {(int)response.StatusCode} {response.StatusCode}\n\nDetalles: {errorJson}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    //response.EnsureSuccessStatusCode();

                    string json = await response.Content.ReadAsStringAsync();
                    dynamic resultado = JsonConvert.DeserializeObject(json);

                    string mensaje = resultado.resultado;
                    bool rostroCoincide = false;

                    if (mensaje == "COINCIDE")
                    {
                        MessageBox.Show("✅ Rostro verificado con éxito", "Resultado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        rostroCoincide = true;
                    }
                    else if (mensaje == "IMAGEN_SOSPECHOSA")
                    {
                        MessageBox.Show("⚠️ Rostro coincide pero imagen sospechosa", "Resultado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        rostroCoincide = false;
                    }
                    else if (mensaje == "NO_COINCIDE")
                    {
                        MessageBox.Show("❌ Rostro no coincide", "Resultado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        rostroCoincide = false;
                    }
                    else
                    {
                        MessageBox.Show("❌ Respuesta inesperada del servidor: " + mensaje, "Resultado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        private async Task ProcesarQRDetectado(string tokenEncriptado)
        {
            try
            {
                clsNegocioRegistro objNegRegistro = new clsNegocioRegistro();

                // Validar que se haya capturado el rostro
                if (fotoCapturada == null)
                {
                    MessageBox.Show("Error: No se pudo capturar el rostro automáticamente.", "Error de Captura");
                    return;
                }

                // Desencriptar token
                string tokenDesencriptado = DecryptData(tokenEncriptado);
                string[] tokenSeparado = tokenDesencriptado.Split('|');

                // Validar formato del token
                if (tokenSeparado.Length < 2)
                {
                    MessageBox.Show("Formato de QR inválido", "Error");
                    return;
                }

                // Validar expiración
                if (!ValidarHora(tokenSeparado[1]))
                {
                    MessageBox.Show("QR Expirado", "Token Expirado");
                    objNegRegistro.MtdEliminarToken(tokenSeparado[0]);
                    return;
                }

                // Verificar si el token ya fue usado
                if (objNegRegistro.MtdCompararToken(tokenEncriptado, tokenSeparado[0]))
                {
                    // Mostrar progreso
                    lblMensajeRostro.Text = "✅ QR válido! Verificando rostro...";
                    lblMensajeRostro.ForeColor = Color.Green;

                    // Procesar registro
                    await RegistrarUpetino(tokenSeparado[0]);
                }
                else
                {
                    MessageBox.Show("El código QR ya fue usado", "QR Usado");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al procesar QR: " + ex.Message, "Error");
                Console.WriteLine("Error al procesar QR detectado: " + ex.Message);
            }
        }

        private async void DetectarQRAutomatico()
        {
            try
            {
                if (frame == null || frame.IsEmpty) return;

                // Convertir frame actual a Bitmap
                Bitmap frameBitmap = frame.ToImage<Bgr, byte>().ToBitmap();

                // Crear una máscara para excluir el área del rostro
                Bitmap frameParaQR = ExcluirAreaRostro(frameBitmap);

                // Intentar detectar QR en toda la imagen excepto el área del rostro
                var result = qrReader.Decode(frameParaQR);

                frameParaQR.Dispose();

                if (result != null)
                {
                    bool rostroCentrado = ValidarRostroCentrado();

                    if (!rostroCentrado)
                    {
                        // QR detectado pero rostro no centrado
                        lblMensajeRostro.Text = "⚠️ QR detectado - Centra tu rostro en el marco verde";
                        lblMensajeRostro.ForeColor = Color.Orange;

                        // Sonido de advertencia
                        System.Media.SystemSounds.Exclamation.Play();

                        frameBitmap.Dispose();
                        return; // No procesar hasta que el rostro esté centrado
                    }

                    procesamientoEnCurso = true;
                    timerDeteccion.Stop();

                    // Mostrar mensaje de QR detectado
                    lblMensajeRostro.Text = "🔍 QR detectado! Capturando rostro...";
                    lblMensajeRostro.ForeColor = Color.Blue;

                    // Agregar sonido de confirmación (opcional)
                    System.Media.SystemSounds.Beep.Play();

                    // Capturar foto del rostro recortado automáticamente
                    fotoCapturada = CapturarRostroRecortado();

                    if (fotoCapturada != null)
                    {
                        MostrarFoto(fotoCapturada, pcbFotoCapturada);
                        lblMensajeRostro.Text = "✅ Rostro capturado! Verificando...";
                        await ProcesarQRDetectado(result.Text);
                    }
                    else
                    {
                        lblMensajeRostro.Text = "❌ Error al capturar rostro";
                        lblMensajeRostro.ForeColor = Color.Red;
                    }

                    // Pausa antes de reactivar detección
                    await Task.Delay(2000);
                    procesamientoEnCurso = false;
                    timerDeteccion.Start();
                }

                frameBitmap.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en detección automática: " + ex.Message);
                procesamientoEnCurso = false;
                if (!timerDeteccion.Enabled)
                    timerDeteccion.Start();
            }
        }


        private Bitmap ExcluirAreaRostro(Bitmap frameOriginal)
        {
            try
            {
                // Crear una copia de la imagen original
                Bitmap frameProcesado = new Bitmap(frameOriginal);

                using (Graphics g = Graphics.FromImage(frameProcesado))
                {
                    // Crear un pincel negro para "bloquear" el área del rostro
                    using (SolidBrush brush = new SolidBrush(Color.Black))
                    {
                        // Expandir ligeramente el área del marco para asegurar que no se detecte QR ahí
                        int margen = 10;
                        Rectangle areaExcluida = new Rectangle(
                            Math.Max(0, marco.X - margen),
                            Math.Max(0, marco.Y - margen),
                            Math.Min(frameOriginal.Width - Math.Max(0, marco.X - margen), marco.Width + (margen * 2)),
                            Math.Min(frameOriginal.Height - Math.Max(0, marco.Y - margen), marco.Height + (margen * 2))
                        );

                        // Rellenar el área del rostro con negro para que no se detecte QR ahí
                        g.FillRectangle(brush, areaExcluida);
                    }
                }

                return frameProcesado;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al excluir área del rostro: " + ex.Message);
                // En caso de error, devolver la imagen original
                return new Bitmap(frameOriginal);
            }
        }


        private async void DetectarQRAutomaticoAlternativo()
        {
            try
            {
                if (frame == null || frame.IsEmpty) return;

                // Convertir frame actual a Bitmap
                Bitmap frameBitmap = frame.ToImage<Bgr, byte>().ToBitmap();

                // Definir múltiples áreas de detección excluyendo el área del rostro
                List<Rectangle> areasDeteccion = CrearAreasDeteccion(frameBitmap.Width, frameBitmap.Height);

                ZXing.Result result = null;

                // Probar detección en cada área
                foreach (Rectangle area in areasDeteccion)
                {
                    try
                    {
                        // Verificar que el área esté dentro de los límites
                        if (area.Width > 0 && area.Height > 0 &&
                            area.X + area.Width <= frameBitmap.Width &&
                            area.Y + area.Height <= frameBitmap.Height)
                        {
                            // Recortar el área específica
                            Bitmap areaRecortada = frameBitmap.Clone(area, frameBitmap.PixelFormat);

                            // Intentar detectar QR
                            result = qrReader.Decode(areaRecortada);

                            areaRecortada.Dispose();

                            if (result != null)
                                break; // QR encontrado, salir del bucle
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al procesar área {area}: {ex.Message}");
                        continue; // Continúar con la siguiente área
                    }
                }

                if (result != null)
                {
                    bool rostroCentrado = ValidarRostroCentrado();

                    if (!rostroCentrado)
                    {
                        // QR detectado pero rostro no centrado
                        lblMensajeRostro.Text = "⚠️ QR detectado - Centra tu rostro en el marco verde";
                        lblMensajeRostro.ForeColor = Color.Orange;
                        System.Media.SystemSounds.Exclamation.Play();
                        frameBitmap.Dispose();
                        return;
                    }

                    procesamientoEnCurso = true;
                    timerDeteccion.Stop();

                    lblMensajeRostro.Text = "🔍 QR detectado! Capturando rostro...";
                    lblMensajeRostro.ForeColor = Color.Blue;
                    System.Media.SystemSounds.Beep.Play();

                    fotoCapturada = CapturarRostroRecortado();

                    if (fotoCapturada != null)
                    {
                        MostrarFoto(fotoCapturada, pcbFotoCapturada);
                        lblMensajeRostro.Text = "✅ Rostro capturado! Verificando...";
                        await ProcesarQRDetectado(result.Text);
                    }
                    else
                    {
                        lblMensajeRostro.Text = "❌ Error al capturar rostro";
                        lblMensajeRostro.ForeColor = Color.Red;
                    }

                    await Task.Delay(2000);
                    procesamientoEnCurso = false;
                    timerDeteccion.Start();
                }

                frameBitmap.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en detección automática alternativa: " + ex.Message);
                procesamientoEnCurso = false;
                if (!timerDeteccion.Enabled)
                    timerDeteccion.Start();
            }
        }


        private void MostrarFrame(object sender, EventArgs e)
        {
            captura.Read(frame);
            if (frame.IsEmpty) return;

            Emgu.CV.Mat frameConGuia = frame.Clone();

            // Detectar rostros
            using (var gray = frame.ToImage<Gray, byte>())
            {
                Rectangle[] rostros = detectorRostro.DetectMultiScale(gray, 1.1, 4);

                // Marco verde para rostro (mantener)
                CvInvoke.Rectangle(frameConGuia, marco, new MCvScalar(0, 255, 0), 2);

                // ELIMINAR: Ya no necesitamos el marco azul para QR
                // El QR ahora se puede detectar en cualquier parte excepto el área verde

                // Texto indicativo actualizado
                CvInvoke.PutText(frameConGuia, "Rostro aqui",
                    new System.Drawing.Point(marco.X, marco.Y - 10),
                    FontFace.HersheySimplex, 0.5, new MCvScalar(0, 255, 0), 1);

                // Agregar texto informativo sobre QR
                CvInvoke.PutText(frameConGuia, "QR: Cualquier posicion (excepto area verde)",
                    new System.Drawing.Point(10, 30),
                    FontFace.HersheySimplex, 0.5, new MCvScalar(255, 255, 255), 1);

                bool rostroDentro = false;

                foreach (var r in rostros)
                {
                    CvInvoke.Rectangle(frameConGuia, r, new MCvScalar(255, 0, 0), 2);

                    if (marco.Contains(r.Location) && marco.Contains(new System.Drawing.Point(r.Right, r.Bottom)))
                    {
                        rostroDentro = true;
                        break;
                    }
                }

                // Actualizar mensaje solo si no está procesando
                if (!procesamientoEnCurso)
                {
                    if (rostroDentro)
                    {
                        lblMensajeRostro.Text = "✅ Rostro OK - Muestra tu QR en cualquier posición";
                        lblMensajeRostro.ForeColor = Color.Green;
                    }
                    else
                    {
                        lblMensajeRostro.Text = "❌ Alinea tu rostro dentro del marco verde";
                        lblMensajeRostro.ForeColor = Color.Orange;
                    }
                }
            }

            pcbCamara.Image?.Dispose();
            pcbCamara.Image = frameConGuia.ToImage<Bgr, byte>().ToBitmap();
        }




        private bool ValidarRostroCentrado()
        {
            try
            {
                if (frame == null || frame.IsEmpty) return false;

                using (var gray = frame.ToImage<Gray, byte>())
                {
                    Rectangle[] rostros = detectorRostro.DetectMultiScale(gray, 1.1, 4);

                    foreach (var rostro in rostros)
                    {
                        // Verificar si el rostro está completamente dentro del marco verde
                        if (marco.Contains(rostro.Location) &&
                            marco.Contains(new System.Drawing.Point(rostro.Right, rostro.Bottom)))
                        {
                            return true; // Rostro encontrado y centrado
                        }
                    }
                }

                return false; // No hay rostro centrado
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al validar rostro: " + ex.Message);
                return false;
            }
        }




        private List<Rectangle> CrearAreasDeteccion(int anchoFrame, int altoFrame)
        {
            List<Rectangle> areas = new List<Rectangle>();

            // Tamaño de cada área de detección
            int anchoArea = 200;
            int altoArea = 200;
            int solapamiento = 50; // Solapamiento entre áreas para no perder QRs en los bordes

            // Crear una cuadrícula de áreas de detección
            for (int y = 0; y < altoFrame - altoArea; y += altoArea - solapamiento)
            {
                for (int x = 0; x < anchoFrame - anchoArea; x += anchoArea - solapamiento)
                {
                    Rectangle area = new Rectangle(x, y, anchoArea, altoArea);

                    // Verificar si el área NO se solapa significativamente con el marco del rostro
                    if (!SeSuperponeConMarcoRostro(area))
                    {
                        areas.Add(area);
                    }
                }
            }

            return areas;
        }

        private bool SeSuperponeConMarcoRostro(Rectangle area)
        {
            // Expandir el marco del rostro para crear una zona de exclusión más amplia
            int margen = 20;
            Rectangle marcoExpandido = new Rectangle(
                marco.X - margen,
                marco.Y - margen,
                marco.Width + (margen * 2),
                marco.Height + (margen * 2)
            );

            // Verificar si hay intersección significativa
            Rectangle interseccion = Rectangle.Intersect(area, marcoExpandido);

            // Si la intersección es más del 30% del área, considerarla como superpuesta
            double porcentajeInterseccion = (double)(interseccion.Width * interseccion.Height) / (area.Width * area.Height);

            return porcentajeInterseccion > 0.3;
        }


    }

}
