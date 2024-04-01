using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Threading;

namespace consoleAPp
{
    public class clientClass
    {
        const int maxBlockSize = 1024; // Максимальный размер сообщения для отправки как единое целое
        public Socket socket;
        public string B;

        //Создание сокета нужного типа
        public clientClass()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public async Task ConnectAsync(string serverIP, int serverPort)
        {
            try
            {
                await socket.ConnectAsync(IPAddress.Parse(serverIP), serverPort);
                Console.WriteLine("Успешное подключение к серверу");

                // Запуск фоновую задачу для приема сообщений
                Task.Run(() => ReceiveMessages());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка подключения: {ex.Message}");
            }
        }

        public async Task SendAsync(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);

            if (data.Length <= maxBlockSize)
            {
                // Если размер сообщения не превышает максимальный размер, отправляем его как единое целое
                await socket.SendAsync(new ArraySegment<byte>(data), SocketFlags.None);
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

                    await socket.SendAsync(new ArraySegment<byte>(chunk), SocketFlags.None);
                }
            }
        }

        public async Task ReceiveKeys()
        {
            byte[] buffer = new byte[maxBlockSize]; // Буфер для приема данных
            Console.WriteLine("123");
            
            while (true)
            {
                
                int bytesRead = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
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
                            bytesRead = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
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
            byte[] buffer = new byte[maxBlockSize]; // Буфер для приема данных

            while (true)
            {
                int bytesRead = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);

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
                            bytesRead = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
                            receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            fullMessage.Append(receivedMessage);
                        }

                        receivedMessage = fullMessage.ToString();
                    }
                }
            }
        }

        public async Task CloseAsync()
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }

    }
}
