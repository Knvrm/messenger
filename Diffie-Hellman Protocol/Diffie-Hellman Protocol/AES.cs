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
        Aes myAes;

        public AES(byte[] key, int keySize)
        {
            byte[] truncatedKey = new byte[16];
            Array.Copy(key, truncatedKey, 16);
            var myAes = Aes.Create();
            myAes.KeySize = keySize;
            myAes.Key = truncatedKey;
            myAes.GenerateIV();
        }

        public byte[] Encrypt(string text)
        {
            byte[] data = Encoding.UTF8.GetBytes(text);
            return Encrypt(data);
        }
        public byte[] Encrypt(byte[] data)
        {
            // Создаем объект для шифрования
            ICryptoTransform encryptor = myAes.CreateEncryptor(myAes.Key, myAes.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                // Записываем вектор инициализации в начало потока
                msEncrypt.Write(myAes.IV, 0, myAes.IV.Length);

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

        public byte[] Decrypt(byte[] encryptedData)
        {
            try
            {
                // Создаем объект для расшифрования
                ICryptoTransform decryptor = myAes.CreateDecryptor(myAes.Key, myAes.IV);

                using (MemoryStream msDecrypt = new MemoryStream())
                {
                    // Создаем объект для расшифрования потока данных
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write))
                    {
                        // Расшифровываем данные и записываем в поток
                        csDecrypt.Write(encryptedData, 0, encryptedData.Length);
                        csDecrypt.FlushFinalBlock();
                    }

                    return msDecrypt.ToArray();
                }

            }
            catch (CryptographicException ex)
            {
                // Обработка исключения
                Console.WriteLine("Возникла криптографическая ошибка: " + ex.Message);
                // Другие действия по обработке исключения, если необходимо
            }
            catch (Exception ex)
            {
                Console.WriteLine("Возникло другое исключение: " + ex.Message);
            }
            return new byte[10]; // Возвращаем пустой массив в случае ошибки
           
        }
        public string DecryptString(byte[] encryptedData)
        {
            return Encoding.UTF8.GetString(Decrypt(encryptedData));
        }
    }
}
