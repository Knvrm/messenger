using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Diffie_Hellman_Protocol
{
    public class ClientClass
    {
        const int maxBlockSize = 1024; // Максимальный размер сообщения для отправки как единое целое
        public TcpClient client;
        public string B;
        public NetworkStream stream;

        //Создание сокета нужного типа
        public ClientClass()
        {
            client = new TcpClient();
        }

        public void ConnectAsync(string serverIP, int serverPort)
        {
            client.ConnectAsync(serverIP, serverPort).Wait();
            stream = client.GetStream();
        }

/*        public byte[] ReadStream()
        {
            var buffer = new List<byte>();
            int bytesRead = 10;
            // считываем данные до конечного символа
            while ((bytesRead = stream.ReadByte()) != -1)
            {
                // добавляем в буфер
                buffer.Add((byte)bytesRead);
            }
            return buffer.ToArray();
        }*/
    }
}
