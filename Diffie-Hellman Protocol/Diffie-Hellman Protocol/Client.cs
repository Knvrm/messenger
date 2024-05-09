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
                Send(client.stream, Encoding.UTF8.GetBytes("AUTH"));
                string login = richTextBox1.Text;
                string password = richTextBox2.Text;
                Send(client.stream, Encoding.UTF8.GetBytes("LOGIN " + login));
                Send(client.stream, Encoding.UTF8.GetBytes("PASSWORD " + login));
                while(true)
                {
                    string msg = ReceiveString(client.stream);
                    if (msg == "SUCCESFUL_AUTH")
                    {
                        Console.WriteLine("Успешная авторизация");
                        Messenger form = new Messenger();
                        form.Show();
                        this.Close();
                        break;
                    }
                    else if (msg == "FAILURE_AUTH")
                    {
                        MessageBox.Show("Неправильно введен логин или пароль",
                            "Ошибка авторизации", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
                        
                }
            }
           
        }

        private async void Client_Load(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                Send(client.stream, "GEN_KEY");
                byte[] data;
                data = Receive(client.stream);
                BigInteger p = new BigInteger(data);

                data = Receive(client.stream);
                BigInteger g = new BigInteger(data);

                Console.WriteLine("Client P: " + p.ToString());
                Console.WriteLine("Client G: " + g.ToString());

                BigInteger a = DiffieHellman.GenerateSecondPublicParam(PrimeNumberUtils.GetBitLength(p));
                BigInteger A = DiffieHellman.CalculateKey(g, a, p);
                Console.WriteLine("Client a:" + a.ToString());
                Console.WriteLine("Client gen A:" + A.ToString());
                Send(client.stream, A);
                
                data = Receive(client.stream);
                BigInteger B = new BigInteger(data);
                Console.WriteLine("Client receiver B:" + B.ToString());

                BigInteger k = DiffieHellman.CalculateKey(B, a, p);
                Console.WriteLine("Client calculate k:" + k.ToString());
                while (true)
                {
                    string msg = ReceiveString(client.stream);
                    if (msg == "SUCCESFUL_GEN")
                    {
                        Console.WriteLine("Успешная генерация ключа");
                        break;
                    }
                    else if (msg == "FAILURE_GEN")
                    {
                        MessageBox.Show("Ключ не был сгенерирован",
                            "Error Diffie-Hellman", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
                }
            });
        }

        private void UpdateRichTextBox(string text)
        {
            if (richTextBox1.InvokeRequired)
            {
                richTextBox1.Invoke(new Action(() =>
                {
                    UpdateRichTextBox(text);
                }));
            }
            else
            {
                richTextBox1.AppendText(text + Environment.NewLine);
            }
        }

        private void Client_FormClosing(object sender, FormClosingEventArgs e)
        {
            Send(client.stream, "CLIENT_CLOSED");
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Send(client.stream, "REGISTRATION");
        }
    }
}
