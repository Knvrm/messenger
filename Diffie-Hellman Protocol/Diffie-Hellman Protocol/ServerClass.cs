using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Numerics;
using static Diffie_Hellman_Protocol.DiffieHellman;
using static Diffie_Hellman_Protocol.NetworkStreamManager;
using System.Data;
using System.Security.Cryptography;


namespace Diffie_Hellman_Protocol
{
    public class ServerClass
    {
        private TcpListener listener;
        static MySqlConnection connection;
        public int BitLength = 128;

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
                TcpClient client = await listener.AcceptTcpClientAsync();

                await Task.Run(() => HandleClient(client));
            }
        }

        private void HandleClient(TcpClient client)
        {
            var stream = client.GetStream();
            int idUser = 0;
            AESAdapter aes = null;
            string text = ReceiveString(stream);
            if(text == "GEN_KEY")
            {
                BigInteger key = GenerateKey(stream);

                if (key != 0)
                {
                    //Console.WriteLine(PrimeNumberUtils.GetBitLength(key));
                    aes = new AESAdapter(key.ToByteArray(), PrimeNumberUtils.GetBitLength(key));// ????? передавать только ключ, битность считать внутри
                    //aes = new AES(key.Take(16).ToArray(), PrimeNumberUtils.GetBitLength(key));
                    Send(stream, "SUCCESFUL_GEN");
                }
                else
                    Send(stream, "FAILURE_GEN");
            }

            while (true)
            {
                text = ReceiveEncryptedText(stream, aes.Key);
                switch (text)
                {
                    case "REGISTRATION":
                        string login1 = ReceiveEncryptedText(stream, aes.Key);
                        if (DBManager.IsUserNameExist(login1, connection))
                            SendEncryptedText(stream, "USER_EXIST", aes); // ??? enum
                        else
                        {
                            SendEncryptedText(stream, "USER_NOT_EXIST", aes);
                            byte[] salt1 = ReceiveEncryptedBytes(stream, aes.Key);
                            byte[] hashedPassword = ReceiveEncryptedBytes(stream, aes.Key);
                            if (DBManager.AddUser(login1, salt1, hashedPassword, connection) == true)
                                SendEncryptedText(stream, "SUCCESFUL_REGISTRATION", aes);
                            else
                                SendEncryptedText(stream, "FAILURE_REGISTRATION", aes);
                        }
                        break;
                    case "AUTH":
                        // Вызов функции авторизации
                        string login = ReceiveEncryptedText(stream, aes.Key);
                        byte[] salt = DBManager.GetUserSalt(login, connection);
                        SendEncryptedBytes(stream, salt, aes);
                        byte[] hashPassword = ReceiveEncryptedBytes(stream, aes.Key);
                        if (DBManager.IsPasswordCorrect(login, hashPassword, salt, connection))
                            idUser = DBManager.GetUserId(login, connection);
                        // Console.WriteLine("server" + Encoding.UTF8.GetString(aes.Encrypt(BitConverter.GetBytes(idUser))));
                        Aes myAes = Aes.Create();
                        SendEncryptedText(stream, idUser.ToString(), aes);
                        break;
                    case "GET_CHATS":
                        if (idUser != 0)
                        {
                            string chats = DBManager.GetChatsByUserId(idUser, connection);
                            if (chats != "")
                            {
                                SendEncryptedText(stream, chats, aes);
                                string chatNames = DBManager.GetChatNamesByIds(chats, connection);
                                SendEncryptedText(stream, chatNames, aes);
                            }
                            else
                                SendEncryptedText(stream, "CHATS_NOT_FOUND", aes);
                        }
                        break;
                    case "GET_CHAT_MESSAGES":
                        //userid не проверяется принадлежности к чату
                        // НЕ ДОВЕРЯТЬ ДАННЫМ ПРИСЛАННЫМ ОТ ПОЛЬЗОВАТЕЛЯ
                        // ВСЕ ДАННЫЕ НУЖНО ПРОВЕРЯТЬ!!!!
                        int ChatId = Int32.Parse(ReceiveEncryptedText(stream, aes.Key));
                        List<Tuple<string, string>> messages = DBManager.GetMessagesWithSenders(ChatId, connection);
                        SendEncryptedText(stream, messages.Count.ToString(), aes);
                        foreach (var message in messages)
                        {
                            SendEncryptedText(stream, message.Item1.ToString(), aes);
                            SendEncryptedText(stream, message.Item2.ToString(), aes);
                        }
                        break;
                    case "SEND_MESSAGE":
                        string msg = ReceiveEncryptedText(stream, aes.Key);
                        string[] parts = msg.Split(' ');

                        // Получаем айди чата, айди пользователя и текст сообщения
                        string chatId = parts[0];
                        string userId = parts[1];
                        string content = string.Join(" ", parts.Skip(2));
                        if (DBManager.AddMessage(chatId, userId, content, connection))
                            SendEncryptedText(stream, "SUCCESFUL_SEND", aes);
                        else
                            SendEncryptedText(stream, "FAILURE_SEND", aes);
                        break;
                    case "GET_ALL_USER_NAMES":
                        string res = DBManager.GetAllUserNames(connection);
                        SendEncryptedText(stream, res, aes);
                        break;
                    /*case "TEST":
                        string data1 = SecurityReceive(stream, aes.Key);
                        Console.WriteLine("Получено сообщение от клиента " + data1);
                        break;*/
                    case "CLIENT_CLOSED":
                        Console.WriteLine("Клиент отключился");
                        stream.Close();
                        client.Close();
                        return;
                    default:
                        break;
                }


            }
        }
        public void SendEncryptedText(NetworkStream stream, string text, AESAdapter aes)
        {
            byte[] data = Encoding.UTF8.GetBytes(text);
            Aes myAes = Aes.Create();
            byte[] encrypted = AESAdapter.EncryptStringToBytes_Aes(text, aes.Key, myAes.IV);

            NetworkStreamManager.SendEncryptedText(stream, encrypted, myAes.IV);
        }
        public BigInteger GenerateKey(NetworkStream stream)
        {
            byte[] data;
            BigInteger[] paramsArray = GenerateFirstPublicParams(BitLength);
            BigInteger p = paramsArray[0], g = paramsArray[1];

            Send(stream, p);
            Send(stream, g);

            data = Receive(stream);
            BigInteger A = new BigInteger(data);

            BigInteger b, B;
            do
            {
                b = DiffieHellman.GenerateSecondPublicParam(BitLength);
                B = DiffieHellman.CalculateKey(g, b, p);
            }
            while (b >= p || PrimeNumberUtils.GetBitLength(B) != BitLength);
            

            Send(stream, B);
            BigInteger k = DiffieHellman.CalculateKey(A, b, p);
            if (PrimeNumberUtils.GetBitLength(k) < BitLength)
                k += BigInteger.Pow(2, BitLength - 1);
            return k;
        }
    }
}
