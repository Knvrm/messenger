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
        public TcpClient client;
        public NetworkStream stream;

        public ClientClass()
        {
            client = new TcpClient();
        }

        public void ConnectAsync(string serverIP, int serverPort)
        {
            client.ConnectAsync(serverIP, serverPort).Wait();
            stream = client.GetStream();
        }
    }
}
