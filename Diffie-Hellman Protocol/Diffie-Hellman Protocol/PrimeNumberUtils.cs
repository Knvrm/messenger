using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Collections;

namespace Diffie_Hellman_Protocol
{
    internal class PrimeNumberUtils
    {
        public static BigInteger GenerateBigInteger(int bit)
        {
            string a = "1";
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            for (int i = 1; i < bit; i++)
            {
                byte[] randomByte = new byte[1];
                rng.GetBytes(randomByte);
                a += Convert.ToString(randomByte[0] & 1);
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
        public static BigInteger GenerateBigInteger2(int bit)
        {
            string a = "";
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            for (int i = 1; i <= bit; i++)
            {
                byte[] randomByte = new byte[1];
                rng.GetBytes(randomByte);
                a += Convert.ToString(randomByte[0] & 1);
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

        public static BigInteger GeneratePrimeNumber(int bit)
        {
            while (true)
            {
                BigInteger x = GenerateBigInteger(bit);

                if (x % 2 == 0)
                    x++;
                //Console.WriteLine((int)BigInteger.Log(x));
                //Console.WriteLine(x.ToString());
                if (MillerRabinTest(x, (int)BigInteger.Log(x)))
                    return x;
            }
        }
        public static int GetBitLength(BigInteger x)
        {
            int bitLength = 0;
            while (x != 0)
            {
                x >>= 1;
                bitLength++;
            }


            return bitLength;
        }
        public static bool MillerRabinTest(BigInteger n, int k)
        {
            if (n <= 1 || n == 4)
                return false;
            if (n <= 3)
                return true;

            // Представляем n - 1 в виде (2^s * d), где d - нечётное
            BigInteger d = n - 1;
            int s = 0;
            while (d % 2 == 0)
            {
                d /= 2;
                s++;
            }

            // Проводим k итераций
            for (int i = 0; i < k; i++)
            {
                // Генерируем случайное число a в диапазоне [2, n - 1]
                BigInteger a = GenerateBigInteger2(GetBitLength(n-1));

                // Вычисляем x = a^d mod n
                BigInteger x = BigInteger.ModPow(a, d, n);

                // Если x равно 1 или n - 1, переходим к следующей итерации
                if (x == 1 || x == n - 1)
                    continue;

                // Повторяем операцию r - 1 раз, где r = s
                for (int r = 1; r < s; r++)
                {
                    x = BigInteger.ModPow(x, 2, n);
                    if (x == 1)
                        return false; // Находим свидетеля непростоты числа
                    if (x == n - 1)
                        break; // Переходим к следующей итерации
                }

                if (x != n - 1)
                    return false; // Находим свидетеля непростоты числа
            }

            // Если для всех k итераций не найдено свидетеля непростоты, вероятность того, что число простое, высока
            return true;
        }
    }
}
