using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Diffie_Hellman_Protocol.DiffieHellman;

namespace Diffie_Hellman_Protocol
{
    public partial class Server : Form
    {
        BigInteger a, b, g, p;
        Server srv;

        public Server()
        {
            InitializeComponent();
            const string ip = "127.0.0.1";
            const int port = 8081;
            srv = new Server(ip, port);
        }

        public async Task StartServer1(Server srv)
        {
            await srv.StartServerAsync();
            richTextBox1.Text += "Сервер запущен!";
        }
        private async void Generate_Click(object sender, EventArgs e)
        {
            await StartServer1(srv);
            int bit = 512;

            BigInteger[] paramsArray = GenerateFirstPublicParams(bit);
            p = paramsArray[0];
            g = paramsArray[1];

            do
                a = DiffieHellman.GenerateSecondPublicParam(bit);
            while (a >= p);

            do
                b = DiffieHellman.GenerateSecondPublicParam(bit);
            while (b >= p);
            InputP.Text = p.ToString();
            InputG.Text = g.ToString();
            InputA.Text = a.ToString();
            InputB.Text = b.ToString();
            srv.clients[0].Send(p.ToByteArray());
        }
       

        private void Count_Click(object sender, EventArgs e)
        {
            BigInteger A = DiffieHellman.CalculateKey(g, a, p);
            OutputA.Text = A.ToString();
            BigInteger B = DiffieHellman.CalculateKey(g, b, p);
            OutputB.Text = B.ToString();
            BigInteger Ka = DiffieHellman.CalculateKey(A, b, p);
            OutputKa.Text = Ka.ToString();
            BigInteger Kb = DiffieHellman.CalculateKey(B, a, p);
            OutputKb.Text = Kb.ToString();

        }
    }
}
