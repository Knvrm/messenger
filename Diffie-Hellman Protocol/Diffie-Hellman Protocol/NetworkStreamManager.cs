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
        public static void SecuritySend(NetworkStream stream, byte[] data, byte[] iv)
        {
            byte[] lengthBytes = BitConverter.GetBytes(iv.Length + 1);
            stream.WriteAsync(lengthBytes, 0, lengthBytes.Length).Wait();

            stream.WriteAsync(iv, 0, iv.Length).Wait();
            
            lengthBytes = BitConverter.GetBytes(data.Length);
            stream.WriteAsync(lengthBytes, 0, lengthBytes.Length).Wait();

            stream.WriteAsync(data, 0, data.Length).Wait();
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

        public static byte[] SecurityReceive(NetworkStream stream, byte[] key)
        {
            byte[] lengthBytes = new byte[sizeof(int)];
            stream.ReadAsync(lengthBytes, 0, lengthBytes.Length).Wait();

            int ivLength = BitConverter.ToInt32(lengthBytes, 0);
            byte[] _iv = new byte[ivLength];
            stream.ReadAsync(_iv, 0, _iv.Length).Wait();
            byte[] iv = new byte[_iv.Length - 1];
            Array.Copy(_iv, 0, iv, 0, iv.Length);

            stream.ReadAsync(lengthBytes, 0, lengthBytes.Length).Wait();
            int dataLength = BitConverter.ToInt32(lengthBytes, 0);

            // Чтение фактических данных
            byte[] data = new byte[dataLength];
            stream.ReadAsync(data, 0, data.Length).Wait();

            string decrypt = AES.DecryptStringFromBytes_Aes(data, key, iv);
            
            Console.WriteLine(decrypt);
            return data;
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

        /*            Console.WriteLine("\nserver data");
            foreach (byte b in data)
            {
                Console.Write(b.ToString() + " "); // Вывод каждого байта как числового значения
            }
            Console.WriteLine("\nserver key");
            foreach (byte b in key)
            {
                Console.Write(b.ToString() + " "); // Вывод каждого байта как числового значения
            }
            Console.WriteLine("\nserver iv");
            foreach (byte b in iv)
            {
                Console.Write(b.ToString() + " "); // Вывод каждого байта как числового значения
            }*/

    }
}
