using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatSSL
{
    public partial class Server : Form
    {
        // Create tcp listener and listen thread
        private TcpListener server;
        private Thread listenThread;
        public Server()
        {
            InitializeComponent();
        }
        public void ServerListen()
        {
            // Create server by using IP endpoint
            IPAddress ipadd = IPAddress.Parse(tbIPServer.Text);
            int port = Convert.ToInt32(tbPort.Text);
            IPEndPoint ipend = new IPEndPoint(ipadd, port);
            server = new TcpListener(ipend);
            // Start the server
            server.Start();
            // Define listen thread
            listenThread = new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        // Create client connect to server
                        TcpClient client = server.AcceptTcpClient();
                        // Get data from client
                        NetworkStream stream = client.GetStream();
                        // Create array to store encoded message
                        byte[] buffer = new byte[1024];
                        // Count bytes in stream
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);
                        // Encode bytes array to get perfect message
                        string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        // Show message
                        AppendText(data);
                    }
                }
                catch (SocketException ex)
                {
                    // Handle socket exception
                    MessageBox.Show("SocketException: " + ex.Message);
                }
            });
            // Start listen thread
            listenThread.Start();
        }

        private void AppendText(string text)
        {
            // Check that does this function run on not main thread
            // If yes, use invoke
            if (InvokeRequired)
            {
                Invoke(new Action<string>(AppendText), text);
                return;
            }
            // Else edit directly
            rtbView.AppendText(text + Environment.NewLine);
        }

        private void ServerClosing(object sender, FormClosingEventArgs e)
        {
            
        }
        

        private void btnListen_Click_1(object sender, EventArgs e)
        {
            ServerListen();
        }

        private void Server_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Stop the server
            server.Stop();
            // Stop listen thread
            if (listenThread != null && listenThread.IsAlive)
            {
                listenThread.Join(); // Wait for the thread to gracefully exit
            }
        }
    }
}
