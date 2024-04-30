using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Eventing.Reader;
using System.Threading;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using static consoleAPp.Sql;
using static consoleAPp.Diffie_Hellman;

namespace consoleAPp
{
    public class serverClass
    {
        const int maxBlockSize = 1024; // Максимальный размер сообщения для отправки как единое целое
        private Socket listener;
        private List<Socket> clients = new List<Socket>();
        static MySqlConnection connection;

        public serverClass(string ipAddress, int port)
        {
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(new IPEndPoint(IPAddress.Parse(ipAddress), port));
        }

        public async Task StartServerAsync()
        {
            listener.Listen(10); // Максимальное количество ожидающих клиентов

            string connectionString = "server=localhost;database=mydb;user=root;password=root";
           /* connection = new MySqlConnection(connectionString);
            connection.ConnectionString = connectionString;
            connection.Open();*/

            while (true)
            {
                Socket client = await listener.AcceptAsync();

                // Обработка каждого клиента в отдельном потоке
                clients.Add(client);
                Task.Run(() => HandleClientAsync(client));
            }

            connection.Close();
            Console.WriteLine("Connection closed.");
        }
        private async Task HandleClientAsync(Socket client)
        {
            // buffer maxblocksize
            // 1. send msg size
            // 2. send msg
            //byte[] buffer = new byte[maxBlockSize];
            // Буфер для больших сообщений
            // don't forget that message could be received not from client
            // buffer overflow ?? будет эта проблема здесь или нет?

            /*int bytesRead = await client.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);*/

            while (true)
            {
                byte[] buffer = new byte[maxBlockSize]; 
                int bytesRead = await client.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine("Received message from client: " + message);
                string[] MessageParts = message.Split(new char[] { ' ' }, 2);
                string login = MessageParts[0];
                string passwd = MessageParts[1];

                if (!Sql.SqlQueryCheckLoginAndPassword(login, passwd, connection))
                {
                    await SendingMessageToClient("auth Неправильно введены учетные данные, попробуйте снова.", client);
                }
                else
                {
                    await SendingMessageToClient($"auth Добро пожаловать {login}:", client);

                    bytesRead = await client.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
                    message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    MessageParts = message.Split(new char[] { ' ' }, 2);
                    string ChatName = MessageParts[0];

                    // Проверка на существование чата
                    if (!Sql.SqlQueryCheckChatName(ChatName, connection))
                        await SendingMessageToClient("errorChatName Данного чата не существует.", client);
                    else
                    {
                        message = MessageParts[1];
                        Console.WriteLine(message);
                        // Отправка сообщения другим пользователям чата
                        foreach (Socket otherClient in clients)
                        {
                            if (otherClient != client && otherClient.Connected) // Проверка, подключен ли клиент
                            {
                                await otherClient.SendAsync(new ArraySegment<byte>
                                    (Encoding.UTF8.GetBytes(message), 0, bytesRead), SocketFlags.None);
                            }
                        }
                    }
                } 
            }
        }

        // Отправка клиенту сообщения
        private async Task SendingMessageToClient(string message, Socket client)
        {
            foreach (Socket otherClient in clients)
            {
                if (otherClient == client && otherClient.Connected)
                {
                    await client.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(message)), SocketFlags.None);
                }
            }
        }

        public void Stop()
        {
            foreach (Socket client in clients)
            {
                client.Shutdown(SocketShutdown.Both);
                client.Close();
            }

            listener.Close();
        }
        
    }
}
