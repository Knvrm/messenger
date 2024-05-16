using MySqlX.XDevAPI;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Utilities;
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
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using static Diffie_Hellman_Protocol.NetworkStreamManager;
using static Mysqlx.Expect.Open.Types.Condition.Types;

namespace Diffie_Hellman_Protocol
{
    public partial class Client : Form
    {
        ClientClass client;
        const string serverIP = "127.0.0.1"; // IP адрес сервера
        const int serverPort = 8081; // Порт сервера
        bool IsKeyGen = false;
        public AES aes;
        public Client()
        {
            InitializeComponent();
            client = new ClientClass();
            client.ConnectAsync(serverIP, serverPort);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            while (!IsKeyGen)
            {
                continue;
            }
            Send(client.stream, "TEST");
            string text = "hello";
            SecuritySend(client.stream, text);
            


            /*if (textBox1.Text == "")
                MessageBox.Show("Введите логин", "Ошибка авторизации", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (textBox2.Text == "")
                MessageBox.Show("Введите пароль", "Ошибка авторизации", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                Send(client.stream, "AUTH");
                string login = textBox1.Text;
                string password = textBox2.Text;
                //login = "roma2003";
                //password = "roma2003";
                SecuritySend(client.stream, login);
                SecuritySend(client.stream, password);
                *//*Send(client.stream, login);
                Send(client.stream, password);*//*
                byte[] data = aes.Decrypt(Receive(client.stream));
                string encrypt = "client" + Encoding.UTF8.GetString(data);
                int idUser = BitConverter.ToInt32(data, 0);
                Console.WriteLine(idUser);
                //Console.WriteLine(idUser);
                //int idUser = BitConverter.ToInt32(Receive(client.stream), 0);
                if (idUser != 0)
                {
                    Console.WriteLine("Успешная авторизация");
                    Messenger form = new Messenger(idUser, client.stream);
                    form.Show();
                    textBox1.Clear();
                    textBox2.Clear();
                    this.Visible = false;
                }
                else
                {
                    MessageBox.Show("Неправильно введен логин или пароль",
                        "Ошибка авторизации", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }*/
        }
        public void SecuritySend(NetworkStream stream, string text)
        {
            byte[] data = Encoding.UTF8.GetBytes(text);
            Aes myAes = Aes.Create();
            byte[] encrypted = AES.EncryptStringToBytes_Aes(text, aes.Key, myAes.IV);
            
            NetworkStreamManager.SecuritySend(stream, encrypted, myAes.IV);
        }

        private async void Client_Load(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                Send(client.stream, "GEN_KEY");
                BigInteger k;

                byte[] data;
                data = Receive(client.stream);
                BigInteger p = new BigInteger(data);

                data = Receive(client.stream);
                BigInteger g = new BigInteger(data);
                int BitLength = PrimeNumberUtils.GetBitLength(p);

                Console.WriteLine("Client P: " + p.ToString());
                Console.WriteLine("Client G: " + g.ToString());
                BigInteger a, A;
                do
                {
                    a = DiffieHellman.GenerateSecondPublicParam(PrimeNumberUtils.GetBitLength(p));
                    A = DiffieHellman.CalculateKey(g, a, p);
                }
                while (a >= p || PrimeNumberUtils.GetBitLength(A) != BitLength);

                Console.WriteLine("Client a:" + a.ToString());
                Console.WriteLine("Client gen A:" + A.ToString());
                Send(client.stream, A);

                data = Receive(client.stream);
                BigInteger B = new BigInteger(data);
                Console.WriteLine("Client receiver B:" + B.ToString());

                k = DiffieHellman.CalculateKey(B, a, p);
                Console.WriteLine("Client calculate k:" + k.ToString());
                if (PrimeNumberUtils.GetBitLength(k) < BitLength)
                    k += BigInteger.Pow(2, BitLength - 1);


                while (true)
                {
                    string msg = ReceiveString(client.stream);
                    if (msg == "SUCCESFUL_GEN")
                    {
                        Console.WriteLine("Успешная генерация ключа");
                        IsKeyGen = true;
                        aes = new AES(k.ToByteArray(), PrimeNumberUtils.GetBitLength(k));
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

        private void UpdateTextBox(string text)
        {
            if (textBox1.InvokeRequired)
            {
                textBox1.Invoke(new Action(() =>
                {
                    UpdateTextBox(text);
                }));
            }
            else
            {
                textBox1.AppendText(text + Environment.NewLine);
            }
        }

        private void Client_FormClosing(object sender, FormClosingEventArgs e)
        {
            Send(client.stream, "CLIENT_CLOSED");
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {            
            Registration reg = new Registration(client.stream);
            reg.Show();
            Visible = false;
        }

        /*Console.WriteLine("client data");
            foreach (byte b in encrypted)
            {
                Console.Write(b.ToString() + " "); // Вывод каждого байта как числового значения
            }
            Console.WriteLine("\nclient key");
            foreach (byte b in aes.Key)
            {
                Console.Write(b.ToString() + " "); // Вывод каждого байта как числового значения
            }
            Console.WriteLine("\nclient iv");
            foreach (byte b in myAes.IV)
            {
                Console.Write(b.ToString() + " "); // Вывод каждого байта как числового значения
            }*/
    }
}
