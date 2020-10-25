using System.IO;
using System.Security.Cryptography;

namespace SafeVaultAlpha.Cryptography
{
    public class MyAES256
    {
        private Aes aes;

        public const int blockSize = 128;
        public const int keySize = 256;

        public MyAES256()
        {
            aes = Aes.Create();
            aes.KeySize = keySize;
            aes.BlockSize = blockSize;
            aes.Mode = CipherMode.CBC;
        }

        public byte[] EncryptAes(byte[] Key, byte[] input, byte[] IV = null)
        {
            if (IV != null)
            {
                aes.IV = IV;
            }
            aes.Key = Key;

            ICryptoTransform encryptor = aes.CreateEncryptor();
            byte[] encrypted;

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        //Write all data to the stream.
                        swEncrypt.Write(input);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }
            return encrypted;
        }

        public byte[] DecryptAes(byte[] Key, byte[] input, byte[] IV = null)
        {
            if (IV != null)
            {
                aes.IV = IV;
            }
            aes.Key = Key;

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            // Create the streams used for decryption.
            using (MemoryStream msDecrypt = new MemoryStream(input))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    csDecrypt.Read(input, 0, input.Length);
                    return msDecrypt.ToArray();
                }
            }
        }
    }
}
