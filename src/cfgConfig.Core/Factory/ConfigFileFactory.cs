using cfgConfig.Core.File;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace cfgConfig.Core.Factory
{
    internal static class ConfigFileFactory
    {
        #region Constants

        private static readonly string HEADER = "cfgConfig-ValidConfigFile"; // 25 bytes
        private static readonly byte[] CONFIG_VERSION = new byte[1] { 0x2 }; // 4 bytes

        #endregion

        /// <summary>
        /// Creates a <see cref="FileStream"/> and writes the byte headers that corresponds to a valid configuration file
        /// </summary>
        /// <param name="configFile"></param>
        /// <returns></returns>
        public static FileStream GetNewConfigurationFile(ConfigurationFile configFile)
        {
            // Ignore if file already exists
            if (configFile.Exists)
                return new FileStream(configFile.Path, FileMode.Open);

            // Get buffer
            byte[] buffer = new byte[30];
            Array.Copy(Encoding.UTF8.GetBytes(HEADER), 0, buffer, 0, 25);
            Array.Copy(CONFIG_VERSION, 0, buffer, 26, 4);

            // Write to a file
            FileStream fs = new FileStream(configFile.Path, FileMode.OpenOrCreate, FileAccess.Write);
            fs.Write(buffer, 0, buffer.Length);

            return fs;
        }

        /// <summary>
        /// Returns true if a file contains the bytes corresponding to a valid configuration file
        /// </summary>
        /// <param name="file">The file to check if its a valid configuration file</param>
        public static bool IsValidConfigurationFile(string file)
        {
            // File not found, return false
            if (!System.IO.File.Exists(file))
                return false;

            // Buffer to store the first 30 bytes
            byte[] buffer = new byte[30];

            // Create stream to read the file
            using (FileStream fs = System.IO.File.Open(file, FileMode.Open))
            {
                // If file is empty or contains less than 30 bytes, return false
                if (fs.Length < 30)
                    return false;

                // Read into the buffer
                fs.Read(buffer, 0, 30);
            }

            // Take first 25 bytes and check if the header matches
            if (Encoding.UTF8.GetString(buffer.Take(25).ToArray()) != HEADER)
                return false; // Return false if no matches

            // If it matches, check the config version
            if (buffer.Skip(25).Take(1).ToArray() != CONFIG_VERSION)
                return false; // Return false if no matches

            // All matches were successful, return valid
            return true;
        }
    }
}