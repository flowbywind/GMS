using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using GMS.Framework.Utility;
using GMS.Core.Config;

namespace GMS.Core.Upload
{
    /// <summary>
    /// 对按需(OnDemand)生成的图片进行拦截生成缩略图
    /// </summary>
    public class ThumbnailHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            //如果304已缓存了，则返回
            if (!String.IsNullOrEmpty(context.Request.Headers["If-Modified-Since"]))
            {
                context.Response.StatusCode = 304;
                context.Response.StatusDescription = "Not Modified";
                return;
            }
            
            var path = context.Request.CurrentExecutionFilePath;

            if (!path.EndsWith(".axd") && !path.StartsWith("/Upload", StringComparison.OrdinalIgnoreCase))
                return;

            //正则从Url里匹配出上传的文件夹目录.....
            var m = Regex.Match(path, @"upload/(.+)/(day_\d+)/thumb/(\d+)_(\d+)_(\d+)\.([A-Za-z]+)\.axd$", RegexOptions.IgnoreCase);

            if (!m.Success)
                return;

            var folder = m.Groups[1].Value;
            var subFolder = m.Groups[2].Value;
            var fileName = m.Groups[3].Value;
            var width = m.Groups[4].Value;
            var height = m.Groups[5].Value;
            var ext = m.Groups[6].Value;

            //如果在配置找不到需要按需生成的，则返回，这样能防止任何人随便敲个尺寸就生成
            string key = string.Format("{0}_{1}_{2}", folder, width, height).ToLower();
            bool isOnDemandSize = UploadConfigContext.ThumbnailConfigDic.ContainsKey(key) && UploadConfigContext.ThumbnailConfigDic[key].Timming == Timming.OnDemand;
            if (!isOnDemandSize)
                return;

            var thumbnailFilePath = string.Format(@"{0}\{1}\Thumb\{2}_{4}_{5}.{3}", folder, subFolder, fileName, ext, width, height);
            thumbnailFilePath = Path.Combine(UploadConfigContext.UploadPath, thumbnailFilePath);

            var filePath = string.Format(@"{0}\{1}\{2}.{3}", folder, subFolder, fileName, ext);
            filePath = Path.Combine(UploadConfigContext.UploadPath, filePath);

            if (!File.Exists(filePath))
                return;

            //如果不存在缩略图，则生成
            if (!File.Exists(thumbnailFilePath))
            {
                var thumbnailFileFolder = string.Format(@"{0}\{1}\Thumb", folder, subFolder);
                thumbnailFileFolder = Path.Combine(UploadConfigContext.UploadPath, thumbnailFileFolder);
                if (!Directory.Exists(thumbnailFileFolder))
                    Directory.CreateDirectory(thumbnailFileFolder);

                ThumbnailHelper.MakeThumbnail(filePath, thumbnailFilePath, UploadConfigContext.ThumbnailConfigDic[key]);
            }

            //缩略图存在了，返回图片字节，并输出304标记
            context.Response.Clear();
            context.Response.ContentType = GetImageType(ext);
            byte[] responseImage = File.ReadAllBytes(thumbnailFilePath);
            context.Response.BinaryWrite(responseImage);

            Set304Cache(context);

            context.Response.Flush();
        }

        private void Set304Cache(HttpContext context)
        {
            context.Response.Cache.SetCacheability(HttpCacheability.Public);
            context.Response.Cache.SetLastModified(DateTime.UtcNow);
            context.Response.AddHeader("If-Modified-Since", DateTime.UtcNow.ToString());
            int maxAge = 86400 * 14; // 14 Day
            context.Response.Cache.SetExpires(DateTime.Now.AddSeconds(maxAge));
            context.Response.Cache.SetMaxAge(new TimeSpan(0, 0, maxAge));
            context.Response.CacheControl = "private";
            context.Response.Cache.SetValidUntilExpires(true);
        }

        private string GetImageType(string ext)
        {
            string contentType = null;

            switch (ext.ToLower())
            {
                case "gif":
                    contentType = "image/gif";
                    break;
                case "jpg":
                case "jpe":
                case "jpeg":
                    contentType = "image/jpeg";
                    break;
                case "bmp":
                    contentType = "image/bmp";
                    break;
                case "tif":
                case "tiff":
                    contentType = "image/tiff";
                    break;
                case "eps":
                    contentType = "application/postscript";
                    break;
                default:
                    break;
            }

            return contentType;
        }
    }
}
