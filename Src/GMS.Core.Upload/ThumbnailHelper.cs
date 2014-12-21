using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using GMS.Framework.Utility;
using GMS.Core.Config;

namespace GMS.Core.Upload
{
    public class ThumbnailHelper
    {
        /// <summary>
        /// 转换如/upload/unit/day_111121/201111211132325858.jpg成/upload/unit/day_111121/Thumb/201111211132325858_400_300.jpg或...400_300.jpg.axd
        /// </summary>
        /// <param name="rawUrl"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static string GetThumbnailUrl(string rawUrl, int width, int height)
        {
            var m = Regex.Match(rawUrl, @"^(.*)/upload/(.+)/(day_\d+)/(\d+)(\.[A-Za-z]+)$", RegexOptions.IgnoreCase);

            if (!m.Success)
                return string.Empty;

            var root = m.Groups[1].Value;
            var folder = m.Groups[2].Value;
            var subFolder = m.Groups[3].Value;
            var fileName = m.Groups[4].Value;
            var ext = m.Groups[5].Value;

            string key = string.Format("{0}_{1}_{2}", folder, width, height).ToLower();
            bool isOnDemandSize = UploadConfigContext.ThumbnailConfigDic.ContainsKey(key) && UploadConfigContext.ThumbnailConfigDic[key].Timming == Timming.OnDemand;
            if (isOnDemandSize)
                ext += ".axd";

            var url = string.Format("{0}/upload/{1}/{2}/thumb/{3}_{4}_{5}{6}", root, folder, subFolder, fileName, width, height, ext);

            return url;
        }

        public static void MakeThumbnail(string originalImagePath, string thumbnailPath, ThumbnailSize size)
        {
            try
            {
                ImageUtil.MakeThumbnail(originalImagePath, thumbnailPath, 
                    size.Width, 
                    size.Height, 
                    size.Mode, 
                    size.AddWaterMarker, 
                    size.WaterMarkerPosition, 
                    size.WaterMarkerPath, 
                    size.Quality);

                Console.WriteLine("生成成功:{0}", thumbnailPath);
            }
            catch (Exception e)
            {
                Console.WriteLine("生成失败，非标准图片:{0}", thumbnailPath);
                //_Logger.Error(string.Format("{0} 生成失败，非标准图片", thumbnailPath), e);
            }
        }

        public static void MakeThumbnail(string originalImagePath, string thumbnailPath, int width, int height, string mode = "Cut", bool isaddwatermark = false, int quality = 88)
        {
            var size = new ThumbnailSize() { Width = width, Height = height, Mode = mode, AddWaterMarker = isaddwatermark, Quality = quality};
            MakeThumbnail(originalImagePath, thumbnailPath, size);
        }
    }
}
