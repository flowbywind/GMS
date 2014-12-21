using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using GMS.Framework.Utility;
using System.Threading.Tasks;
using System.Threading;
using GMS.Core.Config;

namespace GMS.Core.Upload
{
    public class ThumbnailService
    {
        public static void HandleImmediateThumbnail(string filePath,Timming timming  = Timming.Immediate)
        {
            //正则从文件路径里匹配出上传的文件夹目录.....
            var m = Regex.Match(filePath, @"^(.*)\\upload\\(.+)\\(day_\d+)\\(\d+)(\.[A-Za-z]+)$", RegexOptions.IgnoreCase);

            if (!m.Success)
                return;

            var root = m.Groups[1].Value;
            var folder = m.Groups[2].Value;
            var subFolder = m.Groups[3].Value;
            var fileName = m.Groups[4].Value;
            var ext = m.Groups[5].Value;

            foreach (var pair in UploadConfigContext.ThumbnailConfigDic
                .Where(t => t.Key.StartsWith(folder.ToLower() + "_") && t.Value.Timming == timming))
            {
                var size = pair.Value;

                var thumbnailFileFolder = string.Format("{0}\\upload\\{1}\\{2}\\thumb",
                    root, folder, subFolder);

                if (!Directory.Exists(thumbnailFileFolder))
                    Directory.CreateDirectory(thumbnailFileFolder);

                var thumbnailFilePath = string.Format("{0}\\upload\\{1}\\{2}\\thumb\\{3}_{4}_{5}{6}",
                    root, folder, subFolder, fileName, size.Width, size.Height, ext);

                ThumbnailHelper.MakeThumbnail(filePath, thumbnailFilePath, size);
            }
        }

        public static void HandlerLazyThumbnail(int intervalMunites)
        {
            var watcher = new FileSystemWatcher(UploadConfigContext.UploadPath);
            watcher.IncludeSubdirectories = true;
            watcher.Created += (s, e) =>
            {
                HandleImmediateThumbnail(e.FullPath, Timming.Lazy);
            };
            watcher.EnableRaisingEvents = true;
            
            while (true)
            {
                HandlerLazyThumbnail();
                GC.Collect();
                Console.WriteLine("等待 {0} 分钟再重新扫描...........", intervalMunites);
                Thread.Sleep(intervalMunites * 60 * 1000);
            }
        }

        public static void HandlerLazyThumbnail()
        {
            foreach (var group in UploadConfigContext.UploadConfig.UploadFolders)
            {
                var folder = Path.Combine(UploadConfigContext.UploadPath, group.Path);

                if (!Directory.Exists(folder))
                    continue;

                foreach (var dayFolder in Directory.GetDirectories(folder))
                {
                    foreach (var filePath in Directory.GetFiles(dayFolder))
                    {
                        var m = Regex.Match(filePath, @"^(.+\\day_\d+)\\(\d+)(\.[A-Za-z]+)$", RegexOptions.IgnoreCase);

                        if (!m.Success)
                            continue;

                        var root = m.Groups[1].Value;
                        var fileName = m.Groups[2].Value;
                        var ext = m.Groups[3].Value;

                        var thumbnailFileFolder = Path.Combine(dayFolder, "Thumb");

                        if (!Directory.Exists(thumbnailFileFolder))
                            Directory.CreateDirectory(thumbnailFileFolder);

                        //删除配置里干掉的Size对应的缩略图
                        //先不启用，等配置添完了再启用
                        //foreach (var thumbFilePath in Directory.GetFiles(thumbnailFileFolder))
                        //{
                        //    if (!group.ThumbnailSizes.Exists(s => 
                        //        Regex.IsMatch(thumbFilePath, string.Format(@"\\\d+_{0}_{1}+\.[A-Za-z]+$", s.Width, s.Height))))
                        //        File.Delete(thumbFilePath); 
                        //}
                        
                        foreach (var size in group.ThumbnailSizes)
                        {
                            if (size.Timming != Timming.Lazy)
                                continue;

                            var thumbnailFilePath = string.Format("{0}\\thumb\\{1}_{2}_{3}{4}",
                                root, fileName, size.Width, size.Height, ext);

                            if (File.Exists(thumbnailFilePath) && size.IsReplace)
                                File.Delete(thumbnailFilePath);

                            if (!File.Exists(thumbnailFilePath))
                                ThumbnailHelper.MakeThumbnail(filePath, thumbnailFilePath, size);  
                        }
                    }
                }   
            }
        }
    }
}
