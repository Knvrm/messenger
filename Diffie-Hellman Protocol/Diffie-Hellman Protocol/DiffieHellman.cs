using System;
using System.Collections.Generic;
using System.Numerics;
using static Diffie_Hellman_Protocol.PrimeNumberUtils;

namespace Diffie_Hellman_Protocol
{
    internal class DiffieHellman
    {
        private static int bit = 32; // static is bad???
        public static int Bit
        {
            get { return bit; }
            private set { bit = value; }
        }

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

        public static BigInteger GenerateParametr(BigInteger x, BigInteger y, BigInteger p)
        {
            return BigInteger.ModPow(x, y, p);
        }
    }
}
