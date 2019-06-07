using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace cfgConfig.Core.Encryptation
{
    internal static class AES
    {
        /// <summary>
        /// Encrypts a file
        /// </summary>
        public static void EncryptFile(string inputFile, string outputFile, string password)
        {
            // Pin the password
            GCHandle gch = GCHandle.Alloc(password, GCHandleType.Pinned);

            byte[] salt = GenerateRandomSalt();

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

        #region Helper Methods

        /// <summary>
        /// Creates a random salt that will be used to encrypt
        /// </summary>
        /// <returns></returns>
        private static byte[] GenerateRandomSalt()
        {
            // Buffer of 32 bytes
            byte[] data = new byte[32];

            using(RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                for (int i = 0; i < 10; i++)
                    rng.GetBytes(data); // Fill the buffer with the generated data

            return data;
        }

        #endregion
    }
}
