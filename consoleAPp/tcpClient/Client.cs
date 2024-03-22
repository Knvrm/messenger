using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using consoleAPp;
using System.Numerics;

namespace tcpClient
{
    internal class Client
    {
        static async Task Main(string[] args)
        {
            // Создание экземпляр клиентского приложения
            clientClass client = new clientClass();

            // IP адрес и порт сервера
            const string serverIP = "127.0.0.1"; // IP адрес сервера
            const int serverPort = 8081; // Порт сервера
            await client.ConnectAsync(serverIP, serverPort);

            /*BigInteger a, g, prime, A, K;
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
            Console.WriteLine("K " + K.ToString());*/

            Console.WriteLine("Введите \"1\", чтобы отправить сообщение, или \"2\", чтобы закрыть соединение.");

            // Отправка и принятие сообщений
            while (true)
            {
                string userInput = Console.ReadLine(); // Сохранение введенного значение

                if (userInput == "1")
                {
                    Console.WriteLine("Введите ваше сообщение:");
                    client.SendAsync(Console.ReadLine());
                }
                else if (userInput == "2")
                {
                    client.CloseAsync();
                    break; // Выход из цикла
                }
                client.ReceiveMessages();

            }
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

        static private BigInteger GeneratePrimeNumber(int bit, Random rnd)
        {
            BigInteger x = GenerateBigInteger(bit, rnd);

            if (x % 2 == 0)
                x++;

            while (!MillerRabinTest(x, BigInteger.Log(x), bit, rnd))
                x += 2;

            return x;
        }
        static private bool MillerRabinTest(BigInteger n, double k, int array_bit, Random rnd)
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
        static private BigInteger FindPrimitiveRoot(BigInteger p)
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
    // TODO:
    // скинуть код нахождения первообразного корня
    // длинные строки > 256 +
    // возможность обмена сообщениями +
    // threads +
    // через классы +
    // IB +-
}
