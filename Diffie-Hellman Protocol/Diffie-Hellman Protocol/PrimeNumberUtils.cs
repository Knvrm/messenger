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
            int byteCount = (bit + 7) / 8;

            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[byteCount];

            rng.GetBytes(bytes);

            BigInteger result = new BigInteger(bytes);

            return BigInteger.Abs(result);
        }

        public static BigInteger GeneratePrimeNumber(int bit)
        {
            BigInteger x = GenerateBigInteger(bit);
            while(GetBitLength(x) == bit)
            {
                if (x % 2 == 0)
                    x++;

                while (!MillerRabinTest(x, BigInteger.Log(x)))
                    x += 2;
                x = GenerateBigInteger(bit);
            }
            return x;
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
        public static bool MillerRabinTest(BigInteger n, double k)
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
                int bit = GetBitLength(n);
                BigInteger a = GenerateBigInteger(bit);
                while (a < 2 || a > n - 1)
                    a = GenerateBigInteger(bit);
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
    }
}
