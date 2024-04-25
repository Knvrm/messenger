using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace consoleAPp
{
    internal class Diffie_Hellman
    {
        static public BigInteger GenerateBigInteger(int bit, Random rnd)
        {
            string a = "1";

            for (int i = 1; i < bit; i++)
            {
                a += Convert.ToString(rnd.Next(0, 2));
            }
            //BigInteger.Parse()
            char[] a_reverse = a.ToCharArray();
            Array.Reverse(a_reverse);
            a = new string(a_reverse);

            BigInteger b = 0;

            for (int i = 0; i < a.Length; i++)
            {
                b += BigInteger.Pow(2, i) * Convert.ToInt32(Convert.ToString(a[i]));
            }
           
            return BigInteger.Abs(b);//b could be negative?? need to check bit_length
        }

        static public async Task<BigInteger> GenerateParamsAsync(string message, List<Socket> clients)
        {
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

            byte[] buffer1 = B.ToByteArray();

            foreach (Socket otherClient in clients)
            {
                await otherClient.SendAsync(new ArraySegment<byte>(buffer1), SocketFlags.None);
            }
            Console.WriteLine("123");
            return 0;
        }

        static public async Task<BigInteger> GenerateParamsAsync(socket client)
        {
            BigInteger a, g, prime, A, K;
            var rnd = new Random();
            int bit = 32;

            prime = GeneratePrimeNumber(bit, rnd);

            a = GenerateBigInteger(bit, rnd);
            while (a >= prime)
                a = GenerateBigInteger(bit, rnd);

            g = FindPrimitiveRoot(prime);
            A = BigInteger.ModPow(g, a, prime);
            Console.WriteLine("A " + A.ToString());
            Console.WriteLine("a " + a.ToString());
            Console.WriteLine("g " + g.ToString());
            Console.WriteLine("prime " + prime.ToString());

            string keys = g.ToString() + " " + prime.ToString() + " " + A.ToString();

            await client.SendAsync(keys);

            await client.ReceiveKeys();

            K = BigInteger.ModPow(BigInteger.Parse(client.B), a, prime);

            Console.WriteLine();
            Console.WriteLine("K " + K.ToString());
            return 0;
        }

        static public BigInteger GeneratePrimeNumber(int bit, Random rnd)
        {
            BigInteger x = GenerateBigInteger(bit, rnd);

            if (x % 2 == 0)
                x++;

            while (!MillerRabinTest(x, BigInteger.Log(x), bit, rnd))
                x += 2;

            return x;
        }
        static public bool MillerRabinTest(BigInteger n, double k, int array_bit, Random rnd)
        {
            if (n <= 1)
                return false;
            if (n == 2)
                return true;
            if (n % 2 == 0)
                return false;
            BigInteger s = 0, d = n - 1;
            while (d % 2 == 0)
            {
                d /= 2;
                s++;
            }


            for (int i = 0; i < k; i++)
            {
                var rnd2 = new Random();
                BigInteger a = GenerateBigInteger(array_bit, rnd2);
                while (a < 2 || a > n - 1)
                    a = GenerateBigInteger(array_bit, rnd2);
                BigInteger x = BigInteger.ModPow(a, d, n);
                if (x == 1 || x == n - 1)
                    continue;
                for (int j = 0; j < s - 1; j++)
                {
                    x = BigInteger.ModPow(x, 2, n);
                    if (x == 1)
                        return false;
                    if (x == n - 1)
                        break;
                }
                if (x != n - 1)
                    return false;
            }
            return true;
        }
        static public BigInteger FindPrimitiveRoot(BigInteger p)
        {
            List<BigInteger> fact = new List<BigInteger>();
            BigInteger phi = p - 1;
            BigInteger n = phi;
            for (int i = 2; i * i <= n; ++i)
                if (n % i == 0)
                {
                    fact.Add(i);
                    while (n % i == 0)
                        n /= i;
                }
            if (n > 1)
                fact.Add(n);


            for (BigInteger res = BigInteger.Pow(2, 31); res <= p; res++)
            {
                bool ok = true;
                for (int i = 0; i < fact.Count && ok; i++)
                    ok = ok & BigInteger.ModPow(res, phi / fact[i], p) != 1;
                if (ok)
                    return res;
            }
            return -1;
        }
    }
}
