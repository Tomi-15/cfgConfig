using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace cfgConfig.Core.Encryptation
{
    internal static class AES
    {
        #region Constants

        // This constant is used to determine the keysize of the encryption algorithm in bits.
        // We divide this by 8 within the code below to get the equivalent number of bytes.
        private const int KEY_SIZE = 256;

        // This constant determines the number of iterations for the password bytes generation function.
        private const int DERIVATION_ITERATIONS = 1000;

        #endregion

        /// <summary>
        /// Encrypts a file
        /// </summary>
        public static void EncryptFile(string inputFile, string outputFile, string password)
        {
            // Pin the password
            GCHandle gch = GCHandle.Alloc(password, GCHandleType.Pinned);

            byte[] salt = Generate256BitsOfRandomEntropy();

            using(var fsOut = new FileStream(outputFile, FileMode.Create))
            {
                // Get the bytes of the password
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

                using (RijndaelManaged aes = new RijndaelManaged())
                {
                    // Set properties
                    aes.KeySize = 256;
                    aes.BlockSize = 128;
                    aes.Padding = PaddingMode.PKCS7;

                    //http://stackoverflow.com/questions/2659214/why-do-i-need-to-use-the-rfc2898derivebytes-class-in-net-instead-of-directly
                    //"What it does is repeatedly hash the user password along with the salt." High iteration counts.
                    var key = new Rfc2898DeriveBytes(passwordBytes, salt, 50000);
                    aes.Key = key.GetBytes(aes.KeySize / 8);
                    aes.IV = key.GetBytes(aes.BlockSize / 8);

                    //Cipher modes: http://security.stackexchange.com/questions/52665/which-is-the-best-cipher-mode-and-padding-mode-for-aes-encryption
                    aes.Mode = CipherMode.CBC;

                    // Write salt to the begining of the output file
                    fsOut.Write(salt, 0, salt.Length);

                    using(CryptoStream cryptoStream = new CryptoStream(fsOut, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    using(FileStream fsIn = new FileStream(inputFile, FileMode.Open))
                    {
                        // Create a buffer of 1mb so only this amount will allocate in the memory and not the whole file
                        byte[] buffer = new byte[1048576];
                        int read;

                        try
                        {
                            while ((read = fsIn.Read(buffer, 0, buffer.Length)) > 0)
                                cryptoStream.Write(buffer, 0, read);
                        }
                        catch
                        {
                            throw;
                        }
                    }
                }
            }

            // Delete the given password from the memory
            NativeMethods.ZeroMemory(gch.AddrOfPinnedObject(), password.Length * 2);
            gch.Free();
        }

        /// <summary>
        /// Decrypts a file
        /// </summary>
        public static void DecryptFile(string inputFile, string outputFile, string password)
        {
            // Pin the password
            GCHandle gch = GCHandle.Alloc(password, GCHandleType.Pinned);

            byte[] passwordBytes = Encoding.UTF8.GetBytes(password); // Get password bytes
            byte[] salt = new byte[32]; // Get salt

            using(FileStream fsIn = new FileStream(inputFile, FileMode.Open))
            {
                fsIn.Read(salt, 0, salt.Length); // Read the salt

                using(RijndaelManaged aes = new RijndaelManaged())
                {
                    aes.KeySize = 256;
                    aes.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, salt, 50000);
                    aes.Key = key.GetBytes(aes.KeySize / 8);
                    aes.IV = key.GetBytes(aes.BlockSize / 8);
                    aes.Padding = PaddingMode.PKCS7;
                    aes.Mode = CipherMode.CBC;

                    using(CryptoStream cryptoStream = new CryptoStream(fsIn, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    using (FileStream fsOut = new FileStream(outputFile, FileMode.Create))
                    {
                        byte[] buffer = new byte[1048576];
                        int read;

                        try
                        {
                            while ((read = cryptoStream.Read(buffer, 0, buffer.Length)) > 0)
                                fsOut.Write(buffer, 0, read);
                        }
                        catch
                        {
                            throw;
                        }
                    }
                }
            }

            // Delete the given password from the memory
            NativeMethods.ZeroMemory(gch.AddrOfPinnedObject(), password.Length * 2);
            gch.Free();
        }

        /// <summary>
        /// Encrypts text
        /// </summary>
        public static byte[] EncryptString(string inputText, string password)
        {
            // Pin the password
            GCHandle gch = GCHandle.Alloc(password, GCHandleType.Pinned);

            // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
            // so that the same Salt and IV values can be used when decrypting.  
            var saltStringBytes = Generate256BitsOfRandomEntropy();
            var ivStringBytes = Generate256BitsOfRandomEntropy();
            var plainTextBytes = Encoding.UTF8.GetBytes(inputText);
            byte[] cipherTextBytes = new byte[0];

            using (var rfcPassword = new Rfc2898DeriveBytes(password, saltStringBytes, DERIVATION_ITERATIONS))
            {
                var keyBytes = rfcPassword.GetBytes(KEY_SIZE / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();

                                // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                                cipherTextBytes = saltStringBytes;
                                cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                            }
                        }
                    }
                }
            }

            // Delete the given password from the memory
            NativeMethods.ZeroMemory(gch.AddrOfPinnedObject(), password.Length * 2);
            gch.Free();

            return cipherTextBytes;
        }

        /// <summary>
        /// Decrypts data to text
        /// </summary>
        public static string DecryptData(byte[] data, string password)
        {
            // Pin the password
            GCHandle gch = GCHandle.Alloc(password, GCHandleType.Pinned);

            // Get the complete stream of bytes that represent:
            // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
            var cipherTextBytesWithSaltAndIv = data;
            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(KEY_SIZE / 8).ToArray(); // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes. 
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(KEY_SIZE / 8).Take(KEY_SIZE / 8).ToArray(); // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
            var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((KEY_SIZE / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((KEY_SIZE / 8) * 2)).ToArray(); // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.

            string plainText = "";

            using (var rfcPassword = new Rfc2898DeriveBytes(password, saltStringBytes, DERIVATION_ITERATIONS))
            {
                var keyBytes = rfcPassword.GetBytes(KEY_SIZE / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                var plainTextBytes = new byte[cipherTextBytes.Length];
                                var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                plainText = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }

            // Delete the given password from the memory
            NativeMethods.ZeroMemory(gch.AddrOfPinnedObject(), password.Length * 2);
            gch.Free();

            return plainText;
        }

        #region Helper Methods

        /// <summary>
        /// Creates a random salt that will be used to encrypt
        /// </summary>
        /// <returns></returns>
        private static byte[] Generate256BitsOfRandomEntropy()
        {
            // Buffer of 32 bytes
            byte[] data = new byte[32];

            using(RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                rng.GetBytes(data); // Fill the array with cryptographically secure random bytes.

            return data;
        }

        #endregion
    }
}
