using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Threading;
using System.Runtime.CompilerServices;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using MySqlX.XDevAPI;

namespace consoleAPp
{
    public class socket
    {
        const int maxBlockSize = 1024; // Максимальный размер сообщения для отправки как единое целое
        public Socket sckt;
        public string B;

        //Создание сокета нужного типа
        public socket()
        {
            sckt = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public async Task ConnectAsync(string serverIP, int serverPort)
        {
            try
            {
                await sckt.ConnectAsync(IPAddress.Parse(serverIP), serverPort);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка подключения: {ex.Message}");
            }
        }
        

        public async Task ReceiveKeys()
        {
            byte[] buffer = new byte[maxBlockSize]; // Буфер для приема данных
            Console.WriteLine(buffer);
            
            while (true)
            {
                int bytesRead = await sckt.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
                Console.WriteLine(bytesRead);
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
                            bytesRead = await sckt.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
                            receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            fullMessage.Append(receivedMessage);
                        }

                        receivedMessage = fullMessage.ToString();
                        // Передача секретных ключей

                        // Получение ключа

                        BigInteger receivedKey = BigInteger.Parse(receivedMessage);

                        Console.WriteLine("B " + receivedKey.ToString());
                        B = receivedMessage;
                    }
                }
            }

        }

        public async Task ReceiveMessages()
        {
            while (true)
            {
                byte[] buffer = new byte[maxBlockSize]; // Буфер для приема данных

                int bytesRead = await sckt.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);

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
                            bytesRead = await sckt.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
                            receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            fullMessage.Append(receivedMessage);

                        }

                        receivedMessage = fullMessage.ToString();
                    }

                    // Обработка полученного сообщения
                    if (HandleReceivedMessage(receivedMessage))
                        break;
                }
            }
        }

        private bool HandleReceivedMessage(string message)
        {
            string[] MessageParts = message.Split(new char[] { ' ' }, 2);
            string sys = MessageParts[0];
            if(sys == "auth")
                SetText(MessageParts[1]); 
            if(sys == "errorChatName")
                SetText(MessageParts[1]);
            return true ;
        }

        private string text;

        // Метод для присваивания значения переменной
        public void SetText(string value)
        {
            text = value;
        }

        // Метод для получения значения переменной
        public string GetText()
        {
            return text;
        }

        public async Task SendAsync(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);

            if (data.Length <= maxBlockSize)
            {
                // Если размер сообщения не превышает максимальный размер, отправляем его как единое целое
                await sckt.SendAsync(new ArraySegment<byte>(data), SocketFlags.None);
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

                    await sckt.SendAsync(new ArraySegment<byte>(chunk), SocketFlags.None);
                }
            }
        }

        public async Task CloseAsync()
        {
            sckt.Shutdown(SocketShutdown.Both);
            await Task.Run(() => sckt.Close());
        }

    }
}
