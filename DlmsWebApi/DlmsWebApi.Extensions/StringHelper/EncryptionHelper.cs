using System.Security.Cryptography;
using System.Text;

namespace DlmsWebApi.Extensions.StringHelper
{
    /// <summary>
    /// Enterprise-grade helper utility providing AES-256-CBC string encryption and decryption.
    /// Demonstrates secure random IV generation and prepending techniques for semantic security.
    /// </summary>
    public static class EncryptionHelper
    {
        // 256-bit Key (32 bytes) for AES-256
        private static readonly byte[] DefaultKey = Encoding.UTF8.GetBytes("SecurityKeyLibrarySystem12345678"); // Exactly 32 bytes

        /// <summary>
        /// Encrypts a plaintext string using AES-256-CBC with a random Initialization Vector (IV).
        /// </summary>
        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                return plainText;
            }

            using (var aes = Aes.Create())
            {
                aes.Key = DefaultKey;
                aes.GenerateIV(); // Generates a secure random IV to ensure duplicate plaintexts yield unique ciphers

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream())
                {
                    // Prepend the IV to the stream so the decrypter can extract it
                    ms.Write(aes.IV, 0, aes.IV.Length);

                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var writer = new StreamWriter(cs))
                    {
                        writer.Write(plainText);
                    }

                    var encryptedBytes = ms.ToArray();
                    return Convert.ToBase64String(encryptedBytes);
                }
            }
        }

        /// <summary>
        /// Decrypts an AES-256-CBC encrypted Base64 string by extracting the prepended IV.
        /// </summary>
        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
            {
                return cipherText;
            }

            try
            {
                var fullCipherBytes = Convert.FromBase64String(cipherText);

                using (var aes = Aes.Create())
                {
                    aes.Key = DefaultKey;

                    var ivSize = aes.BlockSize / 8; // 16 bytes for AES
                    var iv = new byte[ivSize];
                    var cipherBytes = new byte[fullCipherBytes.Length - ivSize];

                    // Extract IV and raw CipherText bytes from the combined array
                    Buffer.BlockCopy(fullCipherBytes, 0, iv, 0, ivSize);
                    Buffer.BlockCopy(fullCipherBytes, ivSize, cipherBytes, 0, cipherBytes.Length);

                    aes.IV = iv;

                    using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    using (var ms = new MemoryStream(cipherBytes))
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    using (var reader = new StreamReader(cs))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch (Exception)
            {
                // In production, log the exception safely and throw or return generic failure feedback
                return "Decryption failed: invalid key or tampered payload.";
            }
        }
    }
}


