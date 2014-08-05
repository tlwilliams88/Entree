using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Common.Core
{
    public class Crypto
    {
        #region AES Encryption/Decryption with secret key

        private static readonly int _KeySize = 256;
        private static readonly int _BlockSize = 128;
        private static readonly CipherMode _Mode = CipherMode.CBC;
        private static readonly PaddingMode _Padding = PaddingMode.PKCS7;

        private const string Salt = "d5fg4df5sg4ds5fg45sdfg4";
        private const int SizeOfBuffer = 1024 * 8;

        public static string EncryptWithKey(string originalValue, string hexSecretKey)
        {
            string plainIV = hexSecretKey.Substring(0, hexSecretKey.Length / 2);
            var result = GenerateKeyBytes(hexSecretKey, plainIV);
            byte[] key = result.Item1;
            byte[] IV = result.Item2;

            // Encrypt the string to an array of bytes.
            byte[] encrypted = AesEncrypt(originalValue, key, IV);

            return BitConverter.ToString(encrypted).Replace("-", "");
        }

        public static string DecryptWithKey(string encrypted, string hexSecretKey)
        {
            byte[] encryptedBytes = HexStringToByteArray(encrypted);

            string plainIV = hexSecretKey.Substring(0, hexSecretKey.Length / 2);
            var result = GenerateKeyBytes(hexSecretKey, plainIV);
            byte[] key = result.Item1;
            byte[] IV = result.Item2;

            // Decrypt the bytes to a string.
            string decrypted = AesDecrypt(encryptedBytes, key, IV);

            return decrypted;
        }

        public static byte[] EncryptData(byte[] data, string password)
        {
            using (MemoryStream dataStream = new MemoryStream(data))
            {
                // Essentially, if you want to use RijndaelManaged as AES you need to make sure that:
                // 1.The block size is set to 128 bits
                // 2.You are not using CFB mode, or if you are the feedback size is also 128 bits

                var algorithm = new RijndaelManaged { KeySize = 256, BlockSize = 128 };
                var key = new Rfc2898DeriveBytes(password, Encoding.ASCII.GetBytes(Salt));

                algorithm.Key = key.GetBytes(algorithm.KeySize / 8);
                algorithm.IV = key.GetBytes(algorithm.BlockSize / 8);

                using (MemoryStream outputStream = new MemoryStream())
                {
                    using (var encryptedStream = new CryptoStream(outputStream, algorithm.CreateEncryptor(), CryptoStreamMode.Write))
                        CopyStream(dataStream, encryptedStream);

                    return outputStream.ToArray();
                }
            }
        }


        private static void CopyStream(Stream input, Stream output)
        {
            using (output)
            using (input)
            {
                byte[] buffer = new byte[SizeOfBuffer];
                int read;

                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                    output.Write(buffer, 0, read);
            }
        }

        public static byte[] DecryptData(byte[] data, string password)
        {
            //guard clause:
            if (data == null)
                throw new ArgumentNullException("data");

            using (MemoryStream inputStream = new MemoryStream(data))
            {

                // Essentially, if you want to use RijndaelManaged as AES you need to make sure that:
                // 1.The block size is set to 128 bits
                // 2.You are not using CFB mode, or if you are the feedback size is also 128 bits

                var algorithm = new RijndaelManaged { KeySize = 256, BlockSize = 128 };
                var key = new Rfc2898DeriveBytes(password, Encoding.ASCII.GetBytes(Salt));

                algorithm.Key = key.GetBytes(algorithm.KeySize / 8);
                algorithm.IV = key.GetBytes(algorithm.BlockSize / 8);

                try
                {
                    using (MemoryStream outputStream = new MemoryStream())
                    {
                        using (var decryptedStream = new CryptoStream(outputStream, algorithm.CreateDecryptor(), CryptoStreamMode.Write))
                            CopyStream(inputStream, decryptedStream);
                        return outputStream.ToArray();
                    }
                }
                catch (CryptographicException)
                {
                    throw new InvalidDataException("The key used to encrypt the data is different than the key used to decrypt");
                }
            }
        }

        public static byte[] HexStringToByteArray(string hex)
        {
            if (hex.Length % 2 == 1)
                throw new ArgumentException("The binary key cannot have an odd number of digits");

            byte[] arr = new byte[hex.Length >> 1];

            for (int i = 0; i < hex.Length >> 1; ++i)
            {
                arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
            }

            return arr;
        }

        private static int GetHexVal(char hex)
        {
            int val = (int)hex;
            return val - (val < 58 ? 48 : 55);
        }

        private static Tuple<byte[], byte[]> GenerateKeyBytes(string hexKey, string hexIV)
        {
            byte[] key = HexStringToByteArray(hexKey);
            byte[] IV = HexStringToByteArray(hexIV);

            return Tuple.Create(key, IV);
        }

        private static byte[] AesEncrypt(string plainText, byte[] secretKey, byte[] IV)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (secretKey == null || secretKey.Length <= 0)
                throw new ArgumentNullException("secretKey");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            byte[] encrypted;

            using (var alg = new AesManaged())
            {
                alg.BlockSize = _BlockSize;
                alg.KeySize = _KeySize;
                alg.Key = secretKey;
                alg.IV = IV;
                alg.Mode = _Mode;
                alg.Padding = _Padding;

                using (var stream = new MemoryStream())
                {
                    using (var transform = alg.CreateEncryptor(alg.Key, alg.IV))
                    using (var cryptoStream = new CryptoStream(stream, transform, CryptoStreamMode.Write))
                    using (var writer = new StreamWriter(cryptoStream))
                        writer.Write(plainText);

                    encrypted = stream.ToArray();
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        private static string AesDecrypt(byte[] cipherText, byte[] secretKey, byte[] IV)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (secretKey == null || secretKey.Length <= 0)
                throw new ArgumentNullException("secretKey");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            string result = null;

            using (var alg = new AesManaged())
            {
                alg.BlockSize = _BlockSize;
                alg.KeySize = _KeySize;
                alg.Key = secretKey;
                alg.IV = IV;
                alg.Mode = _Mode;
                alg.Padding = _Padding;

                // Create the streams used for decryption.
                using (var stream = new MemoryStream(cipherText))
                {
                    using (var decryptor = alg.CreateDecryptor(alg.Key, alg.IV))
                    using (var decryptStream = new CryptoStream(stream, decryptor, CryptoStreamMode.Read))
                    using (var reader = new StreamReader(decryptStream))
                        result = reader.ReadToEnd();
                }
            }

            return result;
        }

        #endregion

        #region File checksum hash calculation

        public static string CalculateMD5Hash(byte[] dataToHash)
        {
            // Create a new Stringbuilder to collect the bytes and create a string.
            var sBuilder = new StringBuilder();

            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(dataToHash);



                // Loop through each byte of the hashed data 
                // and format each one as a hexadecimal string.
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

            }
            return sBuilder.ToString();
        }

        public static string CalculateMD5Hash(string dataToHash)
        {
            return CalculateMD5Hash(Encoding.UTF8.GetBytes(dataToHash));
        }

        public static string CalculateMD5Hash(Object objectToHash)
        {
            if (objectToHash == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, objectToHash);
            return CalculateMD5Hash(ms.ToArray());
        }

        #endregion
    }

}
