using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace Diffie_Hellman_Protocol
{
    internal class AES
    {
        private readonly byte[] key; // Ключ для шифрования
        private readonly int keySize; // Размер ключа в битах

        public AES(byte[] key, int keySize)
        {
            this.key = key;
            this.keySize = keySize;
        }

        public byte[] Encrypt(string text)
        {
            byte[] data = Encoding.UTF8.GetBytes(text);
            return Encrypt(data);
        }
        public byte[] Encrypt(byte[] data)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.KeySize = keySize;
                aesAlg.GenerateIV(); // Генерируем вектор инициализации

                // Создаем объект для шифрования
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    // Записываем вектор инициализации в начало потока
                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);

                    // Создаем объект для шифрования потока данных
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        // Шифруем данные и записываем в поток
                        csEncrypt.Write(data, 0, data.Length);
                        csEncrypt.FlushFinalBlock();
                    }

                    return msEncrypt.ToArray();
                }
            }
        }

        public byte[] Decrypt(byte[] encryptedData)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.KeySize = keySize;

                // Считываем вектор инициализации из начала зашифрованных данных
                byte[] iv = new byte[aesAlg.IV.Length];
                Array.Copy(encryptedData, 0, iv, 0, iv.Length);
                aesAlg.IV = iv;

                // Создаем объект для расшифрования
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream())
                {
                    // Создаем объект для расшифрования потока данных
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write))
                    {
                        // Расшифровываем данные и записываем в поток
                        csDecrypt.Write(encryptedData, iv.Length, encryptedData.Length - iv.Length);
                        csDecrypt.FlushFinalBlock();
                    }

                    return msDecrypt.ToArray();
                }
            }
        }
        public string DecryptString(byte[] encryptedData)
        {
            return Encoding.UTF8.GetString(Decrypt(encryptedData));
        }
    }
}
