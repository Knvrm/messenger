using MySqlX.XDevAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diffie_Hellman_Protocol
{
    public partial class Client : Form
    {
        ClientClass client;
        public Client()
        {
            InitializeComponent();
            const string serverIP = "127.0.0.1"; // IP адрес сервера
            const int serverPort = 8081; // Порт сервера
            client = new ClientClass();
            Thread.Sleep(5000);
            richTextBox1.Text = client.Connect(serverIP, serverPort);
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            while (true)
            {
                string res = client.ReceiveMessages();
                richTextBox1.Text += res;
            }
        }
    }
}
