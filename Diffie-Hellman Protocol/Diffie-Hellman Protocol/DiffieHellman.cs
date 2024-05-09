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
            
            while(true)
            {
                g = GenerateBigInteger(bit);
                while (g > q - 1) 
                    g = GenerateBigInteger(bit);

                if (BigInteger.ModPow(g, 2, q) != 1)
                {
                    if (BigInteger.ModPow(g, p, q) != 1 && BigInteger.ModPow(g, 2 * p, q) == 1)
                    {
                        return g;
                    }
                }
            }
        }

        public static BigInteger[] GenerateFirstPublicParams(int bit)
        {
            try
            {
                BigInteger[] paramsArray = new BigInteger[2];
                do
                {
                    paramsArray[0] = PrimeNumberUtils.GeneratePrimeNumber(bit - 1);
                }
                while (!MillerRabinTest(2 * paramsArray[0] + 1, 10));
                paramsArray[1] = FindPrimitiveRoot(paramsArray[0]);
                return paramsArray;
            }
            catch(Exception ex) { throw; }
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
