using MySqlX.XDevAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Diffie_Hellman_Protocol
{
    public class NetworkStreamManager
    {
        public static void Send(NetworkStream stream, byte[] data)
        {
            byte[] lengthBytes = BitConverter.GetBytes(data.Length);
            stream.WriteAsync(lengthBytes, 0, lengthBytes.Length).Wait();

            stream.WriteAsync(data, 0, data.Length).Wait();
        }

        public static byte[] ReceiveStream(NetworkStream stream)
        {
            byte[] lengthBytes = new byte[sizeof(int)];
            stream.ReadAsync(lengthBytes, 0, lengthBytes.Length).Wait();
            int dataLength = BitConverter.ToInt32(lengthBytes, 0);

            // Чтение фактических данных
            byte[] data = new byte[dataLength];
            stream.ReadAsync(data, 0, data.Length).Wait();
            return data;
        }
        public static string ReceiveStringFromStream(NetworkStream stream)
        {
            byte[] dataBytes = ReceiveStream(stream);
            return Encoding.UTF8.GetString(dataBytes);
        }
    }
}
