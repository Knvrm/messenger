using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using Org.BouncyCastle.Crypto.Paddings;

namespace Diffie_Hellman_Protocol
{
    public class AESAdapter
    {
        int KeySize;
        public byte[] Key;

        public AESAdapter(byte[] key, int keySize)
        {
            byte[] truncatedKey = new byte[16];
            Array.Copy(key, truncatedKey, 16);
            KeySize = keySize;
            Key = truncatedKey;
        }

        public static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(Key, IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        return msEncrypt.ToArray();
                    }
                }
            }
        }
        public static byte[] EncryptBytesToBytes_Aes(byte[] plainBytes, byte[] Key, byte[] IV)
        {
            // Проверка аргументов.
            if (plainBytes == null || plainBytes.Length == 0)
                throw new ArgumentNullException("plainBytes");
            if (Key == null || Key.Length == 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length == 0)
                throw new ArgumentNullException("IV");

            // Создание объекта Aes
            // с указанным ключом и IV.
            using (Aes aesAlg = Aes.Create())
            {
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(Key, IV);

                // Создание потока для шифрования.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        // Запись всех данных в поток.
                        csEncrypt.Write(plainBytes, 0, plainBytes.Length);
                    }
                    return msEncrypt.ToArray();
                }
            }
        }

        public static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            string plaintext = null;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
        public static byte[] DecryptBytesFromBytes_Aes(byte[] cipherBytes, byte[] Key, byte[] IV)
        {
            // Проверка аргументов.
            if (cipherBytes == null || cipherBytes.Length == 0)
                throw new ArgumentNullException("cipherBytes");
            if (Key == null || Key.Length == 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length == 0)
                throw new ArgumentNullException("IV");

            byte[] decryptedBytes = null;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (MemoryStream msPlainText = new MemoryStream())
                        {
                            csDecrypt.CopyTo(msPlainText);
                            decryptedBytes = msPlainText.ToArray();
                        }
                    }
                }
            }

            return decryptedBytes;
        }
        public static byte[] GenerateIV()
        {
            Aes myAes = Aes.Create();
            myAes.GenerateIV();
            return myAes.IV;
        }
    }
}
