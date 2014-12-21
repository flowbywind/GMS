using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections;
using System.Web;
using System.Web.Routing;
using GMS.Framework.Utility;
using GMS.Core.Upload;

namespace GMS.Web
{
    /// <summary>
    ///  文件服务器分离，需要得到文件服务器上文件的地址
    /// </summary>
    public static class StaticFileHelper
    {
        /// <summary>
        /// 取得静态服务器的网址
        /// 如果是https网站，跨域调用静态资源需要欺骗浏览器如：http://content..../.png 改成 //content..../.png
        /// </summary>
        /// <returns></returns>
        private static string staticServiceUri = null;
        public static string GetStaticServiceUri()
        {
            //var uri = ServiceHelper.GetStaticServiceUri();
            //if (HttpContext.Current.Request.Url.Scheme == "https")
            //    uri = uri.Substring(5);
            //return uri;

            //使用本地图片，而不做资源分离，暂时取本地地址：
            if (staticServiceUri == null)
                staticServiceUri = "http://"  + HttpContext.Current.Request.Url.Authority;

            return staticServiceUri;
        }
        
        /// <summary>
        /// 得到静态文件
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string StaticFile(this UrlHelper helper, string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return "";
            }

            if (path.StartsWith("~"))
                return helper.Content(path);
            else
                return GetStaticServiceUri() + path;
        }

        public static string JsCssFile(this UrlHelper helper, string path)
        {
            var jsAndCssFileEdition = AppSettingsHelper.GetString("JsAndCssFileEdition");
            if (string.IsNullOrEmpty(jsAndCssFileEdition))
                jsAndCssFileEdition = Guid.NewGuid().ToString();

            path += string.Format("?v={0}", jsAndCssFileEdition);

            return helper.StaticFile(path);
        }

        /// <summary>
        /// 得到图片文件，以及缩略图
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="path"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string ImageFile(this UrlHelper helper, string path, string size = null)
        {
            if (string.IsNullOrEmpty(path))
                return helper.StaticFile(@"/content/images/no_picture.jpg"); 
            
            if (size == null)
                return helper.StaticFile(path);
            
            var ext = path.Substring(path.LastIndexOf('.'));
            var head = path.Substring(0, path.LastIndexOf('.'));
            var url = string.Format("{0}{1}_{2}{3}", GetStaticServiceUri(), head, size, ext);
            return url;
        }

        /// <summary>
        /// 得到图片文件，缩略图，根据参数(width:100,height:75)返回如：
        /// http://...../upload/editorial/day_111013/thumb/201110130326326847_100_75.jpg
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="path"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static string ImageFile(this UrlHelper helper, string path, int width, int height, bool containStaticServiceUri = true)
        {
            if (string.IsNullOrEmpty(path))
                return helper.StaticFile(@"/content/images/no_picture.jpg");

            if (width <= 0 || height <= 0)
                return helper.StaticFile(path);

            var thumbnailUrl = ThumbnailHelper.GetThumbnailUrl(path, width, height);
            var url = (containStaticServiceUri ? GetStaticServiceUri() : string.Empty) + thumbnailUrl;

            return url;

        }

        /// <summary>
        /// 得到文件服务器根网址
        /// </summary>
        /// <param name="helper"></param>
        /// <returns></returns>
        public static string StaticFile(this UrlHelper helper)
        {
            return GetStaticServiceUri();
        }
    }
}