using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;
using static Diffie_Hellman_Protocol.PrimeNumberUtils;

namespace Diffie_Hellman_Protocol
{
    internal class DiffieHellman
    {
        public static BigInteger FindPrimitiveRoot(BigInteger p)
        {
            BigInteger q = 2 * p + 1;
            BigInteger g = 2;
            //int bit = GetBitLength(p);

            while (true)
            {
                if (BigInteger.ModPow(g, 2, q) != 1 && BigInteger.ModPow(g, p, q) != 1 && BigInteger.ModPow(g, 2 * p, q) == 1)
                    return g;
                g += 1;
            }
        }
        
        public static BigInteger[] GenerateFirstPublicParams(int bit)
        {
            BigInteger p;
            do
            {
                p = PrimeNumberUtils.GeneratePrimeNumber(bit - 1);
            }
            while (!MillerRabinTest(2 * p + 1, Convert.ToInt32(BigInteger.Log(2 * p + 1))));
            BigInteger q = 2 * p + 1;

            BigInteger g = FindPrimitiveRoot(p);
            return new BigInteger[] { q, g };
        }
        public static BigInteger GenerateSecondPublicParam(int bit)
        {
            return PrimeNumberUtils.GenerateBigInteger(bit);
        }

        public static BigInteger CalculateKey(BigInteger x, BigInteger y, BigInteger p)
        {
            if (p != 0)
                return BigInteger.ModPow(x, y, p);
            else
                return 0;
        }
    }
}
