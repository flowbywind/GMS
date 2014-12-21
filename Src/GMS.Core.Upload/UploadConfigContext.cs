using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using GMS.Framework.Utility;
using GMS.Core.Config;

namespace GMS.Core.Upload
{
    public class UploadConfigContext
    {
        private static readonly object olock = new object();
        public static UploadConfig UploadConfig = CachedConfigContext.Current.UploadConfig;

        static UploadConfigContext()
        {
        }

        public static string uploadPath;
        public static string UploadPath
        {
            get
            {
                if (uploadPath == null)
                {
                    lock (olock)
                    {
                        if (uploadPath == null)
                        {
                            uploadPath = CachedConfigContext.Current.UploadConfig.UploadPath ?? string.Empty;

                            if (HttpContext.Current != null)
                            {
                                var isLocal = Fetch.ServerDomain.IndexOf("guozili", StringComparison.OrdinalIgnoreCase) < 0;
                                if (isLocal || string.IsNullOrEmpty(UploadConfig.UploadPath) || !Directory.Exists(UploadConfig.UploadPath))
                                    uploadPath = HttpContext.Current.Server.MapPath("~/" + "Upload");
                            }
                        }
                    }
                }

                return uploadPath;
            }
        }
        
        private static Dictionary<string, ThumbnailSize> thumbnailConfigDic;
        public static Dictionary<string, ThumbnailSize> ThumbnailConfigDic
        {
            get
            {
                if (thumbnailConfigDic == null)
                {
                    lock (olock)
                    {
                        if (thumbnailConfigDic == null)
                        {
                            thumbnailConfigDic = new Dictionary<string, ThumbnailSize>();

                            foreach (var folder in UploadConfig.UploadFolders)
                            {
                                foreach (var s in folder.ThumbnailSizes)
                                {
                                    var key = string.Format("{0}_{1}_{2}", folder.Path, s.Width, s.Height).ToLower();
                                    if (!thumbnailConfigDic.ContainsKey(key))
                                    {
                                        thumbnailConfigDic.Add(key, s);
                                    }
                                }
                            }
                        }
                    }
                }

                return thumbnailConfigDic;
            }
        }
    }
}
