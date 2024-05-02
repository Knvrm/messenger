using MySqlX.XDevAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Diffie_Hellman_Protocol.NetworkStreamManager;

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
            if(richTextBox1.Text == "")
                MessageBox.Show("Введите логин", "Ошибка авторизации", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (richTextBox2.Text == "")
                MessageBox.Show("Введите пароль", "Ошибка авторизации", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                string login = richTextBox1.Text;
                string password = richTextBox2.Text;
            }
            
        }

        private async void Client_Load(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                string getParams = "GET_DH_P_G";
                byte[] data = Encoding.UTF8.GetBytes(getParams + "\n");
                Send(client.stream, data);

                data = ReceiveStream(client.stream);
                BigInteger p = new BigInteger(data);

                data = ReceiveStream(client.stream);
                BigInteger g = new BigInteger(data);

                Console.WriteLine("Client P: " + p.ToString());
                Console.WriteLine("Client G: " + g.ToString());

                BigInteger a = DiffieHellman.GenerateSecondPublicParam(PrimeNumberUtils.GetBitLength(p));
                BigInteger A = DiffieHellman.CalculateKey(g, a, p);
                Console.WriteLine("Client a:" + a.ToString());
                Console.WriteLine("Client gen A:" + A.ToString());
                data = A.ToByteArray();
                Send(client.stream, data);
                
                data = ReceiveStream(client.stream);
                BigInteger B = new BigInteger(data);
                Console.WriteLine("Client receiver B:" + B.ToString());

                BigInteger k = DiffieHellman.CalculateKey(B, a, p);
                Console.WriteLine("Client calculate k:" + k.ToString());
            });
        }
        
        private void UpdateRichTextBox(string text)
        {
            richTextBox1.Text += text;
        }
    }
}
