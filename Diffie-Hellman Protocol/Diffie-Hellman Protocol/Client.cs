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
        public AESAdapter aes;
        public Client()
        {
            InitializeComponent();
            client = new ClientClass();
            client.ConnectAsync(serverIP, serverPort);
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

                BigInteger a, A;
                do
                {
                    a = DiffieHellman.GenerateSecondPublicParam(PrimeNumberUtils.GetBitLength(p));
                    A = DiffieHellman.CalculateKey(g, a, p);
                }
                while (a >= p || PrimeNumberUtils.GetBitLength(A) != BitLength);


                Send(client.stream, A);

                data = Receive(client.stream);
                BigInteger B = new BigInteger(data);

                k = DiffieHellman.CalculateKey(B, a, p);
                if (PrimeNumberUtils.GetBitLength(k) < BitLength)
                    k += BigInteger.Pow(2, BitLength - 1);

                while (true)
                {
                    string msg = ReceiveString(client.stream);
                    if (msg == "SUCCESFUL_GEN")
                    {
                        Console.WriteLine("Успешная генерация ключа");
                        IsKeyGen = true;
                        aes = new AESAdapter(k.ToByteArray(), PrimeNumberUtils.GetBitLength(k));
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

        private void button1_Click(object sender, EventArgs e)
        {
            if(!IsKeyGen)
            {
                MessageBox.Show("Подождите, идет процесс установления безопасного соединения", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                if (textBox1.Text == "")
                    MessageBox.Show("Введите логин", "Ошибка авторизации", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else if (textBox2.Text == "")
                    MessageBox.Show("Введите пароль", "Ошибка авторизации", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    SendEncryptedText(client.stream, "AUTH");
                    //login = "roma2003";
                    //password = "roma2003";
                    SendEncryptedText(client.stream, textBox1.Text);
                    byte[] salt = ReceiveEncryptedBytes(client.stream, aes.Key);
                    byte[] hashPassword = Hash.HashPassword(textBox2.Text , salt);
                    SendEncryptedBytes(client.stream, hashPassword, aes);

                    string msg = ReceiveEncryptedText(client.stream, aes.Key);
                    int idUser = Int32.Parse(msg);
                    if (idUser != 0)
                    {
                        Console.WriteLine("Успешная авторизация");
                        Messenger form = new Messenger(idUser, client.stream, aes);
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
                }
            }
            
        }
        
        public void SendEncryptedText(NetworkStream stream, string text)
        {
            byte[] data = Encoding.UTF8.GetBytes(text);
            Aes myAes = Aes.Create();
            byte[] encrypted = AESAdapter.EncryptStringToBytes_Aes(text, aes.Key, myAes.IV);

            NetworkStreamManager.SendEncryptedText(stream, encrypted, myAes.IV);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!IsKeyGen)
            {
                MessageBox.Show("Подождите, идет процесс установления безопасного соединения с сервером.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                Registration reg = new Registration(client.stream, aes);
                reg.Show();
                Visible = false;
            }
        }

        private void Client_FormClosing(object sender, FormClosingEventArgs e)
        {
            SendEncryptedText(client.stream, "CLIENT_CLOSED");
        }
    }
}
