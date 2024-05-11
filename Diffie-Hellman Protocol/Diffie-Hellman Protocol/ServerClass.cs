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
using System.IO;
using System.Numerics;
using static Diffie_Hellman_Protocol.DiffieHellman;
using static Diffie_Hellman_Protocol.NetworkStreamManager;
using System.Windows.Forms;
using System.Data;
using static Diffie_Hellman_Protocol.Server;
using System.Diagnostics.Eventing.Reader;


namespace Diffie_Hellman_Protocol
{
    public class ServerClass
    {
        private TcpListener listener;
        public List<NetworkStream> clients = new List<NetworkStream>();
        static MySqlConnection connection;
        public int bit = 128;

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
            int idUser = 0;

            while (true)
            {
                string text = ReceiveString(stream);
                switch (text)
                {
                    case "GEN_KEY":
                        if (GenerateKey(stream)) 
                            Send(stream, "SUCCESFUL_GEN");
                        else
                            Send(stream, "FAILURE_GEN");
                        break;
                    case "AUTH":
                        // Вызов функции авторизации
                        idUser = Authenticate(stream);
                        Send(stream, idUser);
                        break;
                    case "CLIENT_CLOSED":
                        Console.WriteLine("Клиент отключился");
                        stream.Close();
                        client.Close();
                        return;
                    case "REGISTRATION":
                        string login = ReceiveString(stream);
                        string password = ReceiveString(stream);
                        if (DBManager.AddUser(login, password, connection) == true)
                            Send(stream, "SUCCESFUL_REGISTRATION");
                        else
                            Send(stream, "FAILURE_REGISTRATION");
                        break;
                    case "GET_CHATS":
                        if (idUser != 0)
                        {
                            string chats = DBManager.GetChatsByUserId(idUser, connection);
                            Send(stream, chats);
                            string chatNames = DBManager.GetChatNamesByIds(chats, connection);
                            Send(stream, chatNames);
                        }  
                        break;
                    case "GET_CHAT_MESSAGES":
                        byte[] data = Receive(stream);
                        int ChatId = BitConverter.ToInt32(data, 0);
                        List<Tuple<string, string>> messages = DBManager.GetMessagesWithSenders(ChatId, connection);
                        Send(stream, messages.Count);
                        foreach (var message in messages)
                        {
                            //Console.WriteLine($"{message.Item1}: {message.Item2}");
                            Send(stream, message.Item1);
                            Send(stream, message.Item2);
                        }

                        break;
                    default:
                        Console.WriteLine("Получено сообщение от клиента " + text);
                        // Обработка неизвестных сообщений
                        //ProcessUnknownMessage(text);
                        break;
                }


            }
            //client.Close();
        }
        public int Authenticate(NetworkStream stream)
        {
            string login = "", password = "";

            login = ReceiveString(stream);
            password = ReceiveString(stream);
            if (DBManager.IsLoginAndPassword(login, password, connection))
                return DBManager.GetUserId(login, connection);
            else
                return -1;
        }
        public bool GenerateKey(NetworkStream stream)
        {
            byte[] data;
            BigInteger[] paramsArray = GenerateFirstPublicParams(bit);
            BigInteger p = paramsArray[0], g = paramsArray[1];

            Send(stream, p);
            Send(stream, g);

            data = Receive(stream);
            BigInteger A = new BigInteger(data);
            BigInteger b = GenerateSecondPublicParam(PrimeNumberUtils.GetBitLength(p));
            BigInteger B = DiffieHellman.CalculateKey(g, b, p);

            Send(stream, B);
            BigInteger k = DiffieHellman.CalculateKey(A, b, p);
            if (k != 0) 
                return true;
            else
                return false;
            /*Console.WriteLine("Server P: " + p.ToString());
            Console.WriteLine("Server G: " + g.ToString());
            Console.WriteLine("Server received A:" + A.ToString());
            Console.WriteLine("Server b:" + b.ToString());
            Console.WriteLine("Server gen B:" + B.ToString());
            Console.WriteLine("Server calculate k:" + k.ToString());*/


        }
    }
}
