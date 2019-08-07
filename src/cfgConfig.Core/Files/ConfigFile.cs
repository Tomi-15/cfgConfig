using cfgConfig.Core.Encryptation;
using cfgConfig.Core.Engine;
using cfgConfig.Core.Engine.Settings;
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
            Encrypted = Path.GetExtension(FullName) == GlobalSettings.DEFAULT_CRYPTO_EXTENSION;
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
            Decrypt();

            // Read all lines
            string[] lines = File.ReadAllLines(FullName);

            // Encrypt again
            Encrypt();

            return lines;
        }

        /// <summary>
        /// Encrypts the file content
        /// </summary>
        public void Encrypt()
        {
            // Create streams to work with the file
            using(var fs = new FileStream(FullName, FileMode.Open))
            using(StreamReader reader = new StreamReader(fs))
            using (BinaryWriter writer = new BinaryWriter(fs)) 
            {
                string fileContent = reader.ReadToEnd(); // Read file content
                byte[] encrypted = AES.EncryptString(fileContent, "ytHJjaat6NnbyDPHu334Khz3LS8TMGjM"); // Encrypt data
                fs.SetLength(0); // Clear the file
                fs.Flush();
                writer.Write(encrypted); // Write encrypted data to the file
                writer.Flush();
            }
        }

        /// <summary>
        /// Decrypts the file
        /// </summary>
        public string Decrypt()
        {
            string decrypted = "";

            // Create streams to work with the file
            using (var fs = new FileStream(FullName, FileMode.Open))
            using (BinaryReader reader = new BinaryReader(fs))
            {
                byte[] encryptedBuffer = reader.ReadBytes((int)fs.Length); // Read file content
                decrypted = AES.DecryptData(encryptedBuffer, "ytHJjaat6NnbyDPHu334Khz3LS8TMGjM"); // Decrypt data
            }

            return decrypted;
        }

        #endregion
    }
}
