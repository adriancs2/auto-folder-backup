using System;
using System.IO;
using System.Web.Script.Serialization;

namespace AutoFilesBackup.Core
{
    public static class ConfigManager
    {
        private static readonly string ConfigFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "backup_config.json");
        public static BackupConfig LoadConfig()
        {
            if (!File.Exists(ConfigFilePath)) return new BackupConfig();
            try
            {
                var config = new JavaScriptSerializer().Deserialize<BackupConfig>(File.ReadAllText(ConfigFilePath));
                if (config == null) throw new InvalidDataException("Configuration is empty.");
                return config;
            }
            catch (Exception ex)
            {
                throw new InvalidDataException("Cannot read backup_config.json. Existing configuration was retained; correct it before running backups.", ex);
            }
        }
        public static void SaveConfig(BackupConfig config)
        {
            using (new BackupOperationLock())
            {
                BackupEngine.ValidateConfiguration(config);
                AtomicWrite(ConfigFilePath, new JavaScriptSerializer().Serialize(config));
            }
        }
        public static void AtomicWrite(string path, string content)
        {
            string temporary = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
            try
            {
                using (var stream = new FileStream(temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {
                    using (var writer = new StreamWriter(stream, new System.Text.UTF8Encoding(false), 1024, true))
                    { writer.Write(content); writer.Flush(); }
                    stream.Flush(true);
                }
                if (File.Exists(path)) File.Replace(temporary, path, null);
                else File.Move(temporary, path);
            }
            finally { if (File.Exists(temporary)) File.Delete(temporary); }
        }
    }
}
