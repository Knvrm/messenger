using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Diffie_Hellman_Protocol.DiffieHellman;
using static Diffie_Hellman_Protocol.Server;

namespace Diffie_Hellman_Protocol
{
    public partial class Server : Form
    {
        BigInteger a, b, g, p;
        public ServerClass srv;

        public Server()
        {
            InitializeComponent();
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            const string ip = "127.0.0.1";
            const int port = 8081;

            srv = new ServerClass(ip, port);
            await srv.StartServerAsync();
        }

        private void Generate_Click(object sender, EventArgs e)
        {
            int bit = 256;


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
