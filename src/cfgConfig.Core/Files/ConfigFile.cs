using cfgConfig.Core.Encryptation;
using cfgConfig.Core.Engine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace cfgConfig.Core.Files
{
    /// <summary>
    /// Represents a configuration file
    /// </summary>
    public sealed class ConfigFile
    {
        #region Public Properties

        /// <summary>
        /// The name of the file
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The path to the file with its name
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Indicates if the file exists
        /// </summary>
        public bool Exists { get; set; }

        /// <summary>
        /// Indicates if the file is encrypted or not
        /// </summary>
        public bool Encrypted { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new ConfigFile
        /// </summary>
        /// <param name="path">The path to the file</param>
        public ConfigFile(string path)
        {
            FileInfo fi = new FileInfo(path);
            Name = fi.Name;
            FullName = fi.FullName;
            Exists = fi.Exists;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates the file
        /// </summary>
        public void Create()
        {
            try
            {
                using (var fs = new FileStream(FullName, FileMode.Create)) { }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Get the lines of the file
        /// </summary>
        public string[] GetLines()
        {
            // Decrypt the file
            //Decrypt();

            // Read all lines
            string[] lines = File.ReadAllLines(FullName);

            // Encrypt again
            //Encrypt();

            return lines;
        }

        /// <summary>
        /// Encrypts the file
        /// </summary>
        public void Encrypt()
        {
            // Move the file to a temp location
            string tempFilePath = Path.GetTempFileName(); // Get temp location
            try
            {
                // Delete file if already exists
                if (File.Exists(tempFilePath))
                    File.Delete(tempFilePath);

                File.Move(FullName, tempFilePath); // Move the file
                AES.EncryptFile(tempFilePath, FullName, "myPassword"); // Decrypt the file ytHJjaat6NnbyDPHu334Khz3LS8TMGjM
                Logger.LogInfo($"File {Name} encrypted.");
            }
            catch
            {
                throw;
            }

            // Delete the file
            File.Delete(tempFilePath);
        }

        /// <summary>
        /// Decrypts the file
        /// </summary>
        public void Decrypt()
        {
            // Move encrypted file to a temp location
            string tempFilePath = Path.Combine(Path.GetTempPath(), Name + ".encrypted");
            try
            {
                // Delete file if exists
                if (File.Exists(tempFilePath))
                    File.Delete(tempFilePath);

                File.Move(FullName, tempFilePath); // Move the file
                AES.DecryptFile(tempFilePath, FullName, "myPassword"); // Decrypt the file
                Logger.LogInfo($"File {Name} decrypted.");
            }
            catch
            {
                throw;
            }

            // Delete temp file
            File.Delete(tempFilePath);
        }

        #endregion
    }
}
