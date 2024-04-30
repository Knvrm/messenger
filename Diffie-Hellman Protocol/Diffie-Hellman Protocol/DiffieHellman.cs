using System;
using System.Collections.Generic;
using System.Numerics;
using static Diffie_Hellman_Protocol.PrimeNumberUtils;

namespace Diffie_Hellman_Protocol
{
    internal class DiffieHellman
    {
        public static BigInteger FindPrimitiveRoot(BigInteger p)
        {
            BigInteger q = 2 * p + 1;
            BigInteger g = q;
            int bit = GetBitLength(p);
            
            for (int attempt = 0; attempt < 200; attempt++)
            {
                g = GenerateBigInteger(bit);
                while (g > q - 1) 
                    g = GenerateBigInteger(bit);

                if (BigInteger.ModPow(g, 2, q) != 1)
                {
                    if (BigInteger.ModPow(g, p, q) != 1 && BigInteger.ModPow(g, 2 * p, q) != 1)
                    {
                        return g;
                    }
                }
            }
            return -1;
        }

        public static BigInteger[] GenerateFirstPublicParams(int bit)
        {
            BigInteger[] paramsArray = new BigInteger[2];
            do
            {
                paramsArray[0] = PrimeNumberUtils.GeneratePrimeNumber(bit - 1);
                paramsArray[1] = FindPrimitiveRoot(paramsArray[0]);
            }
            while (paramsArray[1] == -1);
            return paramsArray;
        }
        public static BigInteger GenerateSecondPublicParam(int bit)
        {
            return PrimeNumberUtils.GenerateBigInteger(bit);
        }

        public static BigInteger CalculateKey(BigInteger x, BigInteger y, BigInteger p)
        {
            return BigInteger.ModPow(x, y, p);
        }
    }
}
