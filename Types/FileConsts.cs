using System;
using System.Collections.Generic;
using System.Text;

namespace SafeVaultBetta.Types
{
    public class FileConsts
    {
        public readonly static int MinimalFileSize = 64; // %2 == 0 !!!
        public readonly static int VerificationFileMaxLength = 536870912;

        public readonly static string Extenstion = ".dat";
        public readonly static byte[] FileSign = { 0xde, 0xad, 0xbe, 0xef };
    }
}
