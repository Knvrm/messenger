using MySqlX.XDevAPI;
using Org.BouncyCastle.Crypto.Paddings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Numerics;
using System.Security.Cryptography;
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

        public static void SendEncryptedText(NetworkStream stream, byte[] data, byte[] iv)
        {
            byte[] lengthBytes = BitConverter.GetBytes(iv.Length); // Без добавления единицы
            stream.WriteAsync(lengthBytes, 0, lengthBytes.Length).Wait();

            stream.WriteAsync(iv, 0, iv.Length).Wait();

            byte[] lengthBytes2 = BitConverter.GetBytes(data.Length);
            stream.WriteAsync(lengthBytes2, 0, lengthBytes2.Length).Wait();

            stream.WriteAsync(data, 0, data.Length).Wait();
        }
        public static void SendEncryptedBytes(NetworkStream stream, byte[] data, AESAdapter aes)
        {
            byte[] iv = AESAdapter.GenerateIV();
            byte[] data2 = AESAdapter.EncryptBytesToBytes_Aes(data, aes.Key, iv);
            byte[] lengthBytes = BitConverter.GetBytes(iv.Length); // Без добавления единицы
            stream.WriteAsync(lengthBytes, 0, lengthBytes.Length).Wait();

            stream.WriteAsync(iv, 0, iv.Length).Wait();

            byte[] lengthBytes2 = BitConverter.GetBytes(data2.Length);
            stream.WriteAsync(lengthBytes2, 0, lengthBytes2.Length).Wait();

            stream.WriteAsync(data2, 0, data2.Length).Wait();
        }

        public static string ReceiveEncryptedText(NetworkStream stream, byte[] key)
        {
            byte[] lengthBytes = new byte[sizeof(int)];
            stream.ReadAsync(lengthBytes, 0, lengthBytes.Length).Wait();

            int ivLength = BitConverter.ToInt32(lengthBytes, 0);
            byte[] iv = new byte[ivLength];
            stream.ReadAsync(iv, 0, iv.Length).Wait();

            byte[] lengthBytes2 = new byte[sizeof(int)];
            stream.ReadAsync(lengthBytes2, 0, lengthBytes2.Length).Wait();
            int dataLength = BitConverter.ToInt32(lengthBytes2, 0);

            // Чтение фактических данных
            byte[] data = new byte[dataLength];
            stream.ReadAsync(data, 0, data.Length).Wait();
            
            string decrypt = AESAdapter.DecryptStringFromBytes_Aes(data, key, iv);
            return decrypt;
        }
        public static byte[] ReceiveEncryptedBytes(NetworkStream stream, byte[] key)
        {
            byte[] lengthBytes = new byte[sizeof(int)];
            stream.ReadAsync(lengthBytes, 0, lengthBytes.Length).Wait();

            int ivLength = BitConverter.ToInt32(lengthBytes, 0);
            byte[] iv = new byte[ivLength];
            stream.ReadAsync(iv, 0, iv.Length).Wait();

            byte[] lengthBytes2 = new byte[sizeof(int)];
            stream.ReadAsync(lengthBytes2, 0, lengthBytes2.Length).Wait();
            int dataLength = BitConverter.ToInt32(lengthBytes2, 0);

            // Чтение фактических данных
            byte[] data = new byte[dataLength];
            stream.ReadAsync(data, 0, data.Length).Wait();

            byte[] decrypt = AESAdapter.DecryptBytesFromBytes_Aes(data, key, iv);
            return decrypt;
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
