using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Eventing.Reader;
using System.Threading;
using System.Numerics;

namespace consoleAPp
{
    public class serverClass
    {
        const int maxBlockSize = 1024; // Максимальный размер сообщения для отправки как единое целое
        private Socket listener;
        private List<Socket> clients = new List<Socket>();

        public serverClass(string ipAddress, int port)
        {
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(new IPEndPoint(IPAddress.Parse(ipAddress), port));
        }

        public async Task StartServerAsync()
        {
            listener.Listen(10); // Максимальное количество ожидающих клиентов

            while (true)
            {
                Socket client = await listener.AcceptAsync();

                // Обработка каждого клиента в отдельном потоке
                clients.Add(client);
                Task.Run(() => HandleClientAsync(client));
            }
        }
        private async Task HandleClientAsync(Socket client)
        {
            // buffer maxblocksize
            // 1. send msg size
            // 2. send msg
            byte[] buffer = new byte[maxBlockSize]; // Буфер для больших сообщений
                                                    // don't forget that message could be received not from client
                                                    // buffer overflow ?? будет эта проблема здесь или нет?


            /*int bytesRead = await client.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            var rnd = new Random();
            int bit = 32;
            string[] keys = message.Split();
            BigInteger g = BigInteger.Parse(keys[0]);
            BigInteger prime = BigInteger.Parse(keys[1]);
            BigInteger A = BigInteger.Parse(keys[2]);

            BigInteger b = GenerateBigInteger(bit, rnd);
            BigInteger B = BigInteger.ModPow(g, b, prime);
            Console.WriteLine("B " + B.ToString());
            Console.WriteLine("b " + b.ToString());

            Console.WriteLine("A " + A.ToString());
            Console.WriteLine("g " + g.ToString());
            Console.WriteLine("prime " + prime.ToString());

            BigInteger K = BigInteger.ModPow(A, b, prime);
            Console.WriteLine("K " + K.ToString());
            message = B.ToString();

            buffer = Encoding.UTF8.GetBytes(message);
            Console.WriteLine("123");

            foreach (Socket otherClient in clients)
            {
                Console.Out.WriteLine(otherClient);
                await otherClient.SendAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
            }
            Console.WriteLine("123");
*/
            while (true)
            {
                buffer = new byte[maxBlockSize]; // Увеличение размера буфера для больших сообщений
                int bytesRead = await client.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine("Received message from client: " + message);

                // Send the message to all other clients
                foreach (Socket otherClient in clients)
                {
                    if (otherClient != client && otherClient.Connected) // Проверка, подключен ли клиент
                    {
                        await otherClient.SendAsync(new ArraySegment<byte>(buffer, 0, bytesRead), SocketFlags.None);
                    }
                }
            }
        }

        public void Stop()
        {
            foreach (Socket client in clients)
            {
                client.Shutdown(SocketShutdown.Both);
                client.Close();
            }

            listener.Close();
        }
        static private BigInteger GenerateBigInteger(int bit, Random rnd)
        {
            string a = "1";

            for (int i = 1; i < bit; i++)
            {
                a += Convert.ToString(rnd.Next(0, 2));
            }

            char[] a_reverse = a.ToCharArray();
            Array.Reverse(a_reverse);
            a = new string(a_reverse);

            BigInteger b = 0;

            for (int i = 0; i < a.Length; i++)
            {
                b += BigInteger.Pow(2, i) * Convert.ToInt32(Convert.ToString(a[i]));
            }
            return BigInteger.Abs(b);
        }
    }
}
