using System;

namespace cfgConfig.Core.Backups
{
    internal class BackupFile
    {
        /// <summary>
        /// The path to the file.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The date when the backup file was created
        /// </summary>
        public DateTime CreatedDate { get; set; }
    }
}
