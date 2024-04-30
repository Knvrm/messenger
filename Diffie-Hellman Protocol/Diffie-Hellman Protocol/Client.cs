using MySqlX.XDevAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
        const string serverIP = "127.0.0.1"; // IP адрес сервера
        const int serverPort = 8081; // Порт сервера
        public Client()
        {
            InitializeComponent();
            client = new ClientClass();
            client.ConnectAsync(serverIP, serverPort);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            while (true)
            {
                //string res = client.ReceiveMessages();
                //richTextBox1.Text += res;
            }
        }

        private async void Client_Load(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                var response = new List<byte>();
                int bytesRead;
                while (true)
                {
                    // считываем данные до конечного символа
                    while((bytesRead = client.stream.ReadByte()) != '\n')
                    {
                        // добавляем в буфер
                        response.Add((byte)bytesRead);
                    }
                    var translation = Encoding.UTF8.GetString(response.ToArray());
                    UpdateRichTextBox(translation);
                    response.Clear();
                }
            });
        }
        private void UpdateRichTextBox(string text)
        {
            if (richTextBox1.InvokeRequired)
            {
                richTextBox1.Invoke((MethodInvoker)delegate
                {
                    richTextBox1.Text += text;
                });
            }
            else
            {
                richTextBox1.Text += text;
            }
        }
    }
}
