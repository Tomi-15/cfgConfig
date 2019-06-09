using cfgConfig.Core.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace cfgConfig.Core.Backups
{
    internal class BackupManager
    {
        #region Private Members

        string mPath;
        ConfigurationManager mManager;
        List<BackupFile> mOlderBackups;

        #endregion

        public void ConfigureBackupForManager(ConfigurationManager manager, string path)
        {
            // Set properties
            mPath = path;
            mManager = manager;
            mOlderBackups = new List<BackupFile>();

            // Create backups directory if not exists
            try { Directory.CreateDirectory(mPath); } catch { throw; }

            // Get older backups
            GetOlderBackups();

            // Log
            Logger.LogInfo($"Backup system enabled for the manager '{manager.Identifier}'.");
        }

        public void CreateBackups()
        {
            // Get configuration files
            var configFiles = mManager.Implementations.GetConfigImplementations();
            DateTime createdDate = DateTime.Now;
            string backupFilePath = Path.Combine(mPath, $"{Convert.ToBase64String(Encoding.UTF8.GetBytes(createdDate.ToString("dd-MM-yyyy HH:mm:ss")), Base64FormattingOptions.None)}.bkp");
            BackupFile bf = new BackupFile
            {
                CreatedDate = createdDate,
                Path = backupFilePath
            };

            // Compress files
            using(var fs = new FileStream(backupFilePath, FileMode.Create, FileAccess.ReadWrite)) // Create stream to the zip file
            using(var zipArchive = new ZipArchive(fs, ZipArchiveMode.Update)) // Create stream for the zip archive
                // Compress each file
                foreach (var configFile in configFiles)
                    using(StreamWriter streamWriter = new StreamWriter(zipArchive.CreateEntryFromFile(configFile.File.FullName, configFile.File.Name, CompressionLevel.Optimal).Open())) // Write content to the file
                        foreach (var line in configFile.File.GetLines()) // Get its lines
                            streamWriter.WriteLine(line); // Write each line
        }

        public void RestoreLastBackup()
        {
            // Ignore if there are no backups
            if (mOlderBackups.Count <= 0)
                throw new InvalidOperationException("There are no backup files.");

            // Get last backup
            var lastBackup = mOlderBackups.OrderBy(x => x.CreatedDate).First();

            try
            {
                // Decompress files
                string originalPath = Directory.GetParent(mPath).FullName;
                using (ZipArchive zipArchive = ZipFile.OpenRead(lastBackup.Path)) // Get archive
                    foreach (var entry in zipArchive.Entries) // Foreach entry
                        entry.ExtractToFile(Path.Combine(originalPath, entry.Name), true); // Extract to its location
            }
            catch
            {
                throw;
            }

            // Delete backups
            foreach (var backup in mOlderBackups)
                try
                {
                    File.Delete(backup.Path);
                }
                catch
                {
                    throw;
                }
        }

        private void GetOlderBackups()
        {
            // Get older backups
            foreach (var file in Directory.EnumerateFiles(mPath, "*.bkp", SearchOption.TopDirectoryOnly))
            {
                // Get created date
                bool success = DateTime.TryParse(Encoding.UTF8.GetString(Convert.FromBase64String(Path.GetFileNameWithoutExtension(file))), out DateTime createdDate);

                // If failed, continue
                if (!success)
                {
                    Logger.LogError($"Failed trying to read a backup.");
                    continue;
                }

                // Create backup file
                BackupFile bf = new BackupFile
                {
                    CreatedDate = createdDate,
                    Path = file
                };

                // Add to older backups
                mOlderBackups.Add(bf);
            }
        }
    }
}
