using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;
using System.IO;


namespace ChatSSL
{
    public partial class Client : Form
    {
        // Create tcp client
        private TcpClient client;
        private SslStream mySslStream;        
        // Create form server
        private Server server = new Server();
        public Client()
        {
            InitializeComponent();
        }

        private static X509Certificate getServerCert()
        {
            X509Store store = new X509Store(StoreName.My,
                StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);

            X509Certificate2 foundCertificate = null;
            foreach (X509Certificate2 currentCertificate
                in store.Certificates)
            {
                if (currentCertificate.IssuerName.Name
                    != null && currentCertificate.IssuerName.
                        Name.Equals("CN=MySslSocketCertificate"))
                {
                    foundCertificate = currentCertificate;
                    break;
                }
            }


            return foundCertificate;
        }


        private void btnSend_Click(object sender, EventArgs e)
        {
            client = new TcpClient();            
            IPAddress ipadd = IPAddress.Parse(tbIPServer.Text);
            int port = Convert.ToInt32(tbPort.Text);
            IPEndPoint ipend = new IPEndPoint(ipadd, port);
            var clientCertificate = getServerCert();           
            try
            {
                client.Connect(ipend);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
            
            NetworkStream stream = client.GetStream();

            this.mySslStream = new SslStream(client.GetStream());
            this.mySslStream.AuthenticateAsClient("MySslSocketCertificate", new X509CertificateCollection(new X509Certificate[] { clientCertificate }), SslProtocols.Tls12, false);

            string message = tbUserName.Text + ": " + tbMessage.Text;
            rtbView.AppendText(message + "\r\n");            
            Byte[] sendBytes = Encoding.UTF8.GetBytes(message);            
            stream.Write(sendBytes, 0, sendBytes.Length);            
            tbMessage.Text = "";            
            if (client != null)
            {
                client.Close();
            }
        }

        private void Sendimg_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";
            

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Đọc dữ liệu từ tệp hình ảnh
                    byte[] imageData = File.ReadAllBytes(openFileDialog.FileName);

                    // Kết nối đến máy chủ
                    client = new TcpClient(tbIPServer.Text, int.Parse(tbPort.Text));
                    var clientCertificate = getServerCert();
                    this.mySslStream = new SslStream(client.GetStream());
                    this.mySslStream.AuthenticateAsClient("MySslSocketCertificate", new X509CertificateCollection(new X509Certificate[] { clientCertificate }), SslProtocols.Tls12, false);
                    // Gửi dữ liệu hình ảnh qua kết nối TCP
                    NetworkStream stream = client.GetStream();
                    stream.Write(imageData, 0, imageData.Length);

                    MessageBox.Show("Image sent successfully!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while sending the image: " + ex.Message);
                }
                finally
                {
                    client?.Close();
                }
            }
        }
    }
}
