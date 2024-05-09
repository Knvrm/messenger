﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Threading;
using System.IO;
using System.Numerics;
using static Diffie_Hellman_Protocol.DiffieHellman;
using static Diffie_Hellman_Protocol.NetworkStreamManager;
using System.Windows.Forms;
using System.Data;
using static Diffie_Hellman_Protocol.Server;

namespace Diffie_Hellman_Protocol
{
    public class ServerClass
    {
        private TcpListener listener;
        public List<NetworkStream> clients = new List<NetworkStream>();
        static MySqlConnection connection;
        public int bit = 16;

        public ServerClass(string ipAddress, int port)
        {
            listener = new TcpListener(IPAddress.Parse(ipAddress), port);
        }

        public async Task StartServerAsync()
        {
            listener.Start();

            string connectionString = "server=localhost;port=3307;database=mydb;user=root;password=root";
            connection = new MySqlConnection(connectionString);
            connection.ConnectionString = connectionString;
            connection.Open();
            if (connection.State == ConnectionState.Open)
            {
                Console.WriteLine("Подключение к базе данных успешно установлено.");
                // Ваш код работы с базой данных здесь
            }
            else
            {
                Console.WriteLine("Не удалось подключиться к базе данных.");
            }

            while (true)
            {
                // подключение клиента
                TcpClient client = await listener.AcceptTcpClientAsync();

                // обслуживание нового клиента
                await Task.Run(async () => await HandleClientAsync(client));
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            var stream = client.GetStream();
            clients.Add(stream);
            byte[] data;

            string msg = "";
            while (msg == "")
            {
                msg = ReceiveString(stream);
            }
            try
            {
                BigInteger[] paramsArray = GenerateFirstPublicParams(bit);
                BigInteger p = paramsArray[0], g = paramsArray[1];

                Console.WriteLine("Server P: " + p.ToString());
                Console.WriteLine("Server G: " + g.ToString());

                Send(stream, p);
                Send(stream, g);

                data = Receive(stream);
                BigInteger A = new BigInteger(data);
                Console.WriteLine("Server received A:" + A.ToString());

                BigInteger b = GenerateSecondPublicParam(PrimeNumberUtils.GetBitLength(p));
                BigInteger B = DiffieHellman.CalculateKey(g, b, p);
                Console.WriteLine("Server b:" + b.ToString());
                Console.WriteLine("Server gen B:" + B.ToString());
                Send(stream, B);

                BigInteger k = DiffieHellman.CalculateKey(A, b, p);
                Console.WriteLine("Server calculate k:" + k.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Клиент отключился при генерации общего секретного ключа");
            }

            while (true)
            {
                string text = ReceiveString(stream);
                Console.WriteLine("Получено сообщение от клиента" + text);
                
            }
            //client.Close();
        }
    }
}
