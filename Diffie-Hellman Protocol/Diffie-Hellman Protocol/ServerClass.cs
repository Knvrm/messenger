using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Threading;

namespace Diffie_Hellman_Protocol
{
    partial class Server
    {
        const int maxBlockSize = 1024; // Максимальный размер сообщения для отправки как единое целое
        private Socket listener;
        private List<Socket> clients = new List<Socket>();
        static MySqlConnection connection;

        public Server(string ipAddress, int port)
        {
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(new IPEndPoint(IPAddress.Parse(ipAddress), port));
        }

        public async Task StartServerAsync()
        {
            listener.Listen(1); // Максимальное количество ожидающих клиентов
            while (true)
            {
                Socket client = await listener.AcceptAsync();
                clients.Add(client);
                break;
            }

            /*while (true)
            {
                Socket client = listener.Accept();
                // Обработка каждого клиента в отдельном потоке
                //clients.Add(client);
                //Task.Run(() => HandleClientAsync(client));
            }*/

            /*string connectionString = "server=localhost;database=mydb;user=root;password=root";
            connection = new MySqlConnection(connectionString);
            connection.ConnectionString = connectionString;
            connection.Open();*/

            /*

            connection.Close();
            Console.WriteLine("Connection closed.");*/
        }
        public void Send(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);

            if (data.Length <= maxBlockSize)
            {
                // Если размер сообщения не превышает максимальный размер, отправляем его как единое целое
                clients[0].Send(data, SocketFlags.None);
            }
            else
            {
                // Если размер сообщения превышает максимальный размер, разделяем его на части и отправляем по отдельности
                int totalChunks = (int)Math.Ceiling((double)data.Length / maxBlockSize);

                for (int i = 0; i < totalChunks; i++)
                {
                    int offset = i * maxBlockSize;
                    int length = Math.Min(maxBlockSize, data.Length - offset);
                    byte[] chunk = new byte[length];
                    Array.Copy(data, offset, chunk, 0, length);

                    clients[0].Send(chunk, SocketFlags.None);
                }
            }
        }

        public string ReceiveMessages()
        {
            byte[] buffer = new byte[maxBlockSize]; // Буфер для приема данных

            int bytesRead = listener.Receive(buffer, SocketFlags.None);

            if (bytesRead > 0)
            {
                // Получение сообщения 
                string receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                // Если сообщение превышает максимальный размер, оно приходит частями и нужна сборка
                if (bytesRead == buffer.Length)
                {
                    StringBuilder fullMessage = new StringBuilder(receivedMessage);

                    // Продолжаем получать части сообщения
                    while (bytesRead == buffer.Length)
                    {
                        bytesRead = listener.Receive(buffer, SocketFlags.None);
                        receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        fullMessage.Append(receivedMessage);

                    }
                    receivedMessage = fullMessage.ToString();
                    return receivedMessage;
                }
            }
            return "Сообщение не получено";
        }
    }
}
