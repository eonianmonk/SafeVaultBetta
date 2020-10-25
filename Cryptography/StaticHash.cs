using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace SafeVaultAlpha.Cryptography
{
    public static class StaticHash
    {
        public static int SHA1HashLength = 32; // 256/8

        public static byte[] StaticSha256(byte[] input)
        {
            byte[] data;
            using (HashAlgorithm algorithm = SHA256.Create())
            {
                 data = algorithm.ComputeHash(input);
            }
            return data;
        }

    }
}
