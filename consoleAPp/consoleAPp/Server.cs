using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using consoleAPp;

namespace consoleAPp
{

    internal class Server
    {
        static async Task Main(string[] args) 
        {
            const string ip = "127.0.0.1";
            const int port = 8081;

            serverClass server = new serverClass(ip, port);
            await server.StartServerAsync();

        }
    }
}
