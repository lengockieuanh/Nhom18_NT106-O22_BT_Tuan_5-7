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

namespace ChatSSL
{
    public partial class Client : Form
    {
        // Create tcp client
        private TcpClient client;
        // Create form server
        private Server server = new Server();
        public Client()
        {
            InitializeComponent();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            client = new TcpClient();
            // Define IP Endpoint of server
            IPAddress ipadd = IPAddress.Parse(tbIPServer.Text);
            int port = Convert.ToInt32(tbPort.Text);
            IPEndPoint ipend = new IPEndPoint(ipadd, port);
            // Connect client to server by using IP endpoint
            try
            {
                client.Connect(ipend);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
            // Create stream to write message
            NetworkStream stream = client.GetStream();
            string message = tbUserName.Text + ": " + tbMessage.Text;
            rtbView.AppendText(message + "\r\n");
            // Encode message from string to bytes
            Byte[] sendBytes = Encoding.UTF8.GetBytes(message);
            // Write message into stream
            stream.Write(sendBytes, 0, sendBytes.Length);
            // Message textbox will be empty then
            tbMessage.Text = "";
            // Dispose client but the connection is still kept
            if (client != null)
            {
                client.Close();
            }
        }
    }
}
