using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Threading;
using System.IO;
using System.Numerics;
using static Diffie_Hellman_Protocol.DiffieHellman;
using static Diffie_Hellman_Protocol.NetworkStreamManager;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Diffie_Hellman_Protocol
{
    public class ServerClass
    {
        const int maxBlockSize = 1024; // Максимальный размер сообщения для отправки как единое целое
        private TcpListener listener;
        public List<NetworkStream> clients = new List<NetworkStream>();
        static MySqlConnection connection;
        public int bit = 256;

        public ServerClass(string ipAddress, int port)
        {
            listener = new TcpListener(IPAddress.Parse(ipAddress), port);
        }

        public async Task StartServerAsync()
        {
            listener.Start();
            
            while (true)
            {
                // подключение клиента
                TcpClient client = await listener.AcceptTcpClientAsync();

                // обслуживание нового клиента
                await Task.Run(async () => await HandleClientAsync(client));
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            var stream = client.GetStream();
            clients.Add(stream);
            byte[] data;

            string msg = "";
            while(msg == "")
            {
                msg = ReceiveStringFromStream(stream);
            }
            
            BigInteger[] paramsArray = GenerateFirstPublicParams(bit);
            BigInteger p = paramsArray[0], g = paramsArray[1];

            Console.WriteLine("Server P: " + p.ToString());
            Console.WriteLine("Server G: " + g.ToString());

            data = p.ToByteArray();
            Send(stream, data);

            data = g.ToByteArray();
            Send(stream, data);

            data = ReceiveStream(stream);
            BigInteger A = new BigInteger(data);
            Console.WriteLine("Server received A:" + A.ToString());

            BigInteger b = GenerateSecondPublicParam(PrimeNumberUtils.GetBitLength(p));
            BigInteger B = DiffieHellman.CalculateKey(g, b, p);
            Console.WriteLine("Server b:" + b.ToString());
            Console.WriteLine("Server gen B:" + b.ToString());
            data = B.ToByteArray();
            Send(stream, data);

            BigInteger k = DiffieHellman.CalculateKey(A, b, p);
            Console.WriteLine("Server calculate k:" + k.ToString());


            client.Close();
        }

    }

    /*string connectionString = "server=localhost;database=mydb;user=root;password=root";
            connection = new MySqlConnection(connectionString);
            connection.ConnectionString = connectionString;
            connection.Open();*/
}
