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
            int byteCount = (bit + 7) / 8; // Вычисляем количество байтов на основе битовой длины

            byte[] bytes = new byte[byteCount];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

            do
            {
                rng.GetBytes(bytes); // Генерируем случайные байты
                bytes[byteCount - 1] &= (byte)(0xFF >> (8 - (bit % 8))); // Обрезаем лишние биты
            }
            while (bytes[0] == 0); // Проверяем, что старший байт не равен нулю

            BigInteger result = new BigInteger(bytes); // Преобразуем байты в BigInteger

            return BigInteger.Abs(result); // Возвращаем абсолютное значение числа
        }

        public static BigInteger GeneratePrimeNumber(int bit)
        {
            while (true)
            {
                BigInteger x = GenerateBigInteger(bit);

                if (x % 2 == 0)
                    x++;

                if (MillerRabinTest(x, 80))
                    return x;
            }
        }
        public static int GetBitLength(BigInteger x)
        {
            int bitLength = 0;
            while(x != 0)
            {
                x >>= 1;
                bitLength++;
            }
    

            return bitLength;
        }
        public static bool MillerRabinTest(BigInteger n, int k)
        {
            if (n <= 1)
                return false;
            if (n == 2)
                return true;
            if (n % 2 == 0)
                return false;

            BigInteger d = n - 1;
            int s = 0;
            while (d % 2 == 0)
            {
                d /= 2;
                s++;
            }

            for (int i = 0; i < k; i++)
            {
                BigInteger a = GenerateBigInteger(GetBitLength(n)); // Случайное число a
                if (a < 2)
                    a = 2;
                if (a >= n)
                    a = n - 1;

                BigInteger x = BigInteger.ModPow(a, d, n);
                if (x == 1 || x == n - 1)
                    continue;
                bool probablePrime = false;
                for (int j = 0; j < s - 1; j++)
                {
                    x = BigInteger.ModPow(x, 2, n);
                    if (x == 1)
                        return false;
                    if (x == n - 1)
                    {
                        probablePrime = true;
                        break;
                    }
                }
                if (!probablePrime)
                    return false;
            }
            return true;
        }
    }
}
