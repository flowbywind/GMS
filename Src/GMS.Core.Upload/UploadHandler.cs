using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using GMS.Core.Config;

namespace GMS.Core.Upload
{
    public abstract class UploadHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public virtual string FileInputName
        {
            get { return "filedata"; }
        }

        public string UploadPath
        {
            get { return UploadConfigContext.UploadPath; }
        }

        public int MaxFilesize
        {
            //10M 
            get { return 10971520; }
        }

        public virtual string[] AllowExt
        {
            get { return new string[] { "txt", "rar", "zip", "jpg", "jpeg", "gif", "png", "swf"}; }
        }

        public virtual string[] ImageExt
        {
            get { return new string[] { "jpg", "jpeg", "gif", "png" }; }
        }

        public abstract string GetResult(string localFileName, string uploadFilePath, string err);

        public abstract void OnUploaded(HttpContext context, string filePath);

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Charset = "UTF-8";

            byte[] file;
            string localFileName = string.Empty;
            string err = string.Empty;
            string subFolder = string.Empty;
            string fileFolder = string.Empty;
            string filePath = string.Empty; ;

            var disposition = context.Request.ServerVariables["HTTP_CONTENT_DISPOSITION"];
            if (disposition != null)
            {
                // HTML5上传
                file = context.Request.BinaryRead(context.Request.TotalBytes);
                localFileName = Regex.Match(disposition, "filename=\"(.+?)\"").Groups[1].Value;// 读取原始文件名
            }
            else
            {
                HttpFileCollection filecollection = context.Request.Files;
                HttpPostedFile postedfile = filecollection.Get(this.FileInputName);

                // 读取原始文件名
                localFileName = Path.GetFileName(postedfile.FileName);

                // 初始化byte长度.
                file = new Byte[postedfile.ContentLength];

                // 转换为byte类型
                System.IO.Stream stream = postedfile.InputStream;
                stream.Read(file, 0, postedfile.ContentLength);
                stream.Close();

                filecollection = null;
            }

            var ext = localFileName.Substring(localFileName.LastIndexOf('.') + 1).ToLower();

            if (file.Length == 0)
                err = "无数据提交";
            else if (file.Length > this.MaxFilesize)
                err = "文件大小超过" + this.MaxFilesize + "字节";
            else if (!AllowExt.Contains(ext))
                err = "上传文件扩展名必需为：" + string.Join(",", AllowExt);
            else
            {
                var folder = context.Request["subfolder"] ?? "default";
                var uploadFolderConfig = UploadConfigContext.UploadConfig.UploadFolders.FirstOrDefault(u => string.Equals(folder, u.Path, StringComparison.OrdinalIgnoreCase));
                var dirType = uploadFolderConfig == null ? DirType.Day : uploadFolderConfig.DirType;

                //根据配置里的DirType决定子文件夹的层次（月，天，扩展名）
                switch (dirType)
                {
                    case DirType.Month:
                        subFolder = "month_" + DateTime.Now.ToString("yyMM");
                        break;
                    case DirType.Ext:
                        subFolder = "ext_" + ext;
                        break;
                    case DirType.Day:
                        subFolder = "day_" + DateTime.Now.ToString("yyMMdd");
                        break;
                }

                fileFolder = Path.Combine(UploadConfigContext.UploadPath,
                    folder,
                    subFolder
                    );

                filePath = Path.Combine(fileFolder,
                    string.Format("{0}{1}.{2}", DateTime.Now.ToString("yyyyMMddhhmmss"), new Random(DateTime.Now.Millisecond).Next(10000), ext)
                    );

                if (!Directory.Exists(fileFolder))
                    Directory.CreateDirectory(fileFolder);

                var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                fs.Write(file, 0, file.Length);
                fs.Flush();
                fs.Close();

                //是图片，即使生成对应尺寸
                if (ImageExt.Contains(ext))
                    ThumbnailService.HandleImmediateThumbnail(filePath);

                this.OnUploaded(context, filePath);
            }

            file = null;
            context.Response.Write(this.GetResult(localFileName, filePath, err));
            context.Response.End();
        }
    }
}
