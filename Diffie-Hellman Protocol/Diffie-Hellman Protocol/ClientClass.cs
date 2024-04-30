using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Diffie_Hellman_Protocol
{
    internal class ClientClass
    {
        const int maxBlockSize = 1024; // Максимальный размер сообщения для отправки как единое целое
        public Socket sckt;
        public string B;

        //Создание сокета нужного типа
        public ClientClass()
        {
            sckt = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public string Connect(string serverIP, int serverPort)
        {
            try
            {
                sckt.Connect(IPAddress.Parse(serverIP), serverPort);
                return "Успешное подключение к серверу";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public void SendAsync(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);

            if (data.Length <= maxBlockSize)
            {
                // Если размер сообщения не превышает максимальный размер, отправляем его как единое целое
                sckt.Send(data, SocketFlags.None);
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

                    sckt.Send(chunk, SocketFlags.None);
                }
            }
        }
        public string ReceiveMessages()
        {
            while (true)
            {
                byte[] buffer = new byte[maxBlockSize]; // Буфер для приема данных

                int bytesRead = sckt.Receive(buffer, SocketFlags.None);

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
                            bytesRead = sckt.Receive(buffer, SocketFlags.None);
                            receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            fullMessage.Append(receivedMessage);

                        }

                        receivedMessage = fullMessage.ToString();
                    }
                }
            }
        }
    }
}
