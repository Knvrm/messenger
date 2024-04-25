using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Diffie_Hellman_Protocol.DiffieHellman;

namespace Diffie_Hellman_Protocol
{
    public partial class Form1 : Form
    {
        BigInteger a, b, g, p;
        public Form1()
        {
            InitializeComponent();
        }

        private void Generate_Click(object sender, EventArgs e)
        {
            a = PrimeNumberUtils.GenerateBigInteger(1024);
            b = PrimeNumberUtils.GenerateBigInteger(1024);
            InputB.Text = b.ToString();
            p = PrimeNumberUtils.GeneratePrimeNumber(1023);
            g = DiffieHellman.FindPrimitiveRoot(p);
            while (g == -1)
            {
                p = PrimeNumberUtils.GeneratePrimeNumber(1023);
                g = DiffieHellman.FindPrimitiveRoot(p);
            }
            InputP.Text = p.ToString();
            InputG.Text = g.ToString();
            while (a >= p)
                a = PrimeNumberUtils.GenerateBigInteger(1024);

            InputA.Text = a.ToString();
            
        }
       

        private void Count_Click(object sender, EventArgs e)
        {
            BigInteger A = DiffieHellman.GenerateParametr(g, a, p);
            OutputA.Text = A.ToString();
            BigInteger B = DiffieHellman.GenerateParametr(g, b, p);
            OutputB.Text = B.ToString();
            BigInteger Ka = DiffieHellman.GenerateParametr(A, b, p);
            OutputKa.Text = Ka.ToString();
            BigInteger Kb = DiffieHellman.GenerateParametr(B, a, p);
            OutputKb.Text = Kb.ToString();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // Ваш код, для которого вы хотите засечь время выполнения
            BigInteger primitiveRoot = FindPrimitiveRoot(p);

            stopwatch.Stop();
            richTextBox1.Text = ("Old: " + stopwatch.ElapsedTicks + " ticks ");

            Stopwatch stopwatch2 = new Stopwatch();
            stopwatch2.Start();

            primitiveRoot = FindPrimitiveRoot(p);

            stopwatch2.Stop();
            richTextBox1.Text += ("New: " + stopwatch2.ElapsedTicks + " ticks");
        }
    }
}
