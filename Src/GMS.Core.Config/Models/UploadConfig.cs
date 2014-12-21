using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using GMS.Framework.Utility;

namespace GMS.Core.Config
{
    [Serializable]
    public class UploadConfig : ConfigFileBase
    {
        public UploadConfig()
        {
        }


        public List<UploadFolder> UploadFolders { get; set; }
        [XmlAttribute("UploadPath")]
        public string UploadPath { get; set; }
  
    }

    [Serializable]
    public class UploadFolder
    {
        public UploadFolder()
        {
            this.Path = "Default";
            this.DirType = DirType.Day;
        }

        public List<ThumbnailSize> ThumbnailSizes { get; set; }
        [XmlAttribute("Path")]
        public string Path { get; set; }
        [XmlAttribute("DirType")]
        public DirType DirType { get; set; }
    }

    [Serializable]
    public class ThumbnailSize
    {
        public ThumbnailSize()
        {
            this.Quality = 88;
            this.Mode = "Cut";
            this.Timming = Timming.Lazy;
            this.WaterMarkerPosition = ImagePosition.Default;
        }

        [XmlAttribute("Width")]
        public int Width { get; set; }
        [XmlAttribute("Height")]
        public int Height { get; set; }
        [XmlAttribute("Quality")]
        public int Quality { get; set; }
        [XmlAttribute("AddWaterMarker")]
        public bool AddWaterMarker { get; set; }
        [XmlAttribute("WaterMarkerPosition")]
        public ImagePosition WaterMarkerPosition { get; set; }
        [XmlAttribute("WaterMarkerPath")]
        public string WaterMarkerPath { get; set; }
        [XmlAttribute("Mode")]
        public string Mode { get; set; }
        [XmlAttribute("Timming")]
        public Timming Timming { get; set; }
        [XmlAttribute("IsReplace")]
        public bool IsReplace { get; set; }
    }

    public enum DirType
    {
        //按天
        Day = 1,
        //按月份
        Month = 2,
        //按扩展名
        Ext = 3
    }

    [Flags]
    public enum Timming
    {
        //延迟用工具生成
        Lazy = 1,
        //上传后即时生成
        Immediate = 2,
        //请求图片时按需生成
        OnDemand = 4
    }

    /*
    此配置对应的Upload.Config内容注释如下：
    <!--UploadPath=""为空时则取应用程序当前目录下的Upload文件夹-->
    <UploadConfig UploadPath="E:\upload">
      <UploadFolders>
        <!--DirType="Day|Month|Ext"子文件夹按什么分组-->
        <UploadFolder Path="Unit" DirType="Day">
          <ThumbnailSizes>
            <!--Size使用如下：
              <ThumbnailSize Width="500" Height="300" Quality="75" Mode="Cut" Timming="OnDemand" IsReplace="false" AddWaterMarker="true" WaterMarkerPosition="Default" WaterMarkerPath="watermarker.png"/>
              Quality：质量，不填默认为"88"
              IsReplace：取代，不填默认为"false"，主要用于，想让某个尺寸重新生成，就会覆盖取代同文件，生成完成后，再改回来为"false"，不然每次都循环扫描覆盖取代耗性能
              Mode：切图方式，不填默认为"Cut"，有："HW"://指定高宽缩放（可能变形）"W"://指定宽，高按比例 "H"://指定高，宽按比例 "Cut"://指定高宽裁减（不变形）"Fit"://不超出尺寸，比它小就不截了，不留白，大就缩小到最佳尺寸，主要为手机用
              Timming：生成时机，不填默认为"Lazy"，有："Lazy"延迟用工具生成  "Immediate"上传后即时生成   "OnDemand"请求图片时按需生成
              AddWaterMarker：是否加水印，不填默认为"false"，"true"时下面的配置才有用：
              WaterMarkerPosition：水印位置，不填默认为"Default"，有"Default"离左上角很近 "LeftTop" "LeftBottom" "RightTop" "RigthBottom"
              WaterMarkerPath：水印图片，不填默认为"watermarker.png"，为当前程序根目录下的FileName
            -->
            <ThumbnailSize Width="500" Height="300" AddWaterMarker="true" Quality="75" Timming="OnDemand" />
            <ThumbnailSize Width="300" Height="300" Timming="Immediate" />
            <ThumbnailSize Width="200" Height="200" Timming="Lazy" AddWaterMarker="true" IsReplace="true" />
          </ThumbnailSizes>
        </UploadFolder>
      </UploadFolders>
    </UploadConfig>
    */
}
