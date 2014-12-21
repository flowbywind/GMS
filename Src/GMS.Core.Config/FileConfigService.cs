using System;
using System.Collections.Generic;
using System.IO;

namespace GMS.Core.Config
{
    /// <summary>
    /// 配置以文件形式保存在使用目录下的Config，可以实现DBConfigService保存到数据库里去
    /// </summary>
    public class FileConfigService : IConfigService
    {
        private readonly string configFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config");
        
        public string GetConfig(string fileName)
        {
            if (!Directory.Exists(configFolder))
                Directory.CreateDirectory(configFolder);

            var configPath = GetFilePath(fileName);
            if (!File.Exists(configPath))
                return null;
            else
                return File.ReadAllText(configPath);
        }

        public void SaveConfig(string fileName, string content)
        {
            var configPath = GetFilePath(fileName);
            File.WriteAllText(configPath, content);
        }

        public string GetFilePath(string fileName)
        {
            var configPath = string.Format(@"{0}\{1}.xml", configFolder, fileName);
            return configPath;
        }
    }
}
