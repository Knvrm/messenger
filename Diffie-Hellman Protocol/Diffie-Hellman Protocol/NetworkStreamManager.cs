using MySqlX.XDevAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Diffie_Hellman_Protocol
{
    public class NetworkStreamManager
    {
        public static void Send(NetworkStream stream, byte[] data)
        {
            try
            {
                byte[] lengthBytes = BitConverter.GetBytes(data.Length);
                stream.WriteAsync(lengthBytes, 0, lengthBytes.Length).Wait();

                stream.WriteAsync(data, 0, data.Length).Wait();
            }
            catch
            {
                Console.WriteLine("Ошибка отправки");
            }
        }
        public static void Send(NetworkStream stream, BigInteger x)
        {
            byte[] data = x.ToByteArray();
            Send(stream, data);
        }
        public static void Send(NetworkStream stream, string text)
        {
            byte[] data = Encoding.UTF8.GetBytes(text);
            Send(stream, data);
        }
        public static void Send(NetworkStream stream, int x)
        {
            byte[] data = BitConverter.GetBytes(x);
            Send(stream, data);
        }

        public static byte[] Receive(NetworkStream stream)
        {
            byte[] lengthBytes = new byte[sizeof(int)];
            stream.ReadAsync(lengthBytes, 0, lengthBytes.Length).Wait();
            int dataLength = BitConverter.ToInt32(lengthBytes, 0);

            // Чтение фактических данных
            byte[] data = new byte[dataLength];
            stream.ReadAsync(data, 0, data.Length).Wait();
            return data;
        }
        public static string ReceiveString(NetworkStream stream)
        {
            byte[] dataBytes = Receive(stream);
            return Encoding.UTF8.GetString(dataBytes);
        }

    }
}
