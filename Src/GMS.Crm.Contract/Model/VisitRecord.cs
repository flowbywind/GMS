using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using GMS.Framework.Utility;
using GMS.Framework.Contract;

namespace GMS.Crm.Contract
{
    [Table("VisitRecord")]
    public partial class VisitRecord : ModelBase
    {
        public int UserId { get; set; }
        [StringLength(50)]
        public string Username { get; set; }

        public int VisitWay { get; set; }
        public int FollowLevel { get; set; }
        public int FollowStep { get; set; }
        public int ProductType { get; set; }

        public int CityId { get; set; }
        public int AreaId { get; set; }
        
        /// <summary>
        /// 成交可能性
        /// </summary>
        public int Probability { get; set; }

        /// <summary>
        /// 沟通细节
        /// </summary>
       [StringLength(400, ErrorMessage = "沟通细节不能超过400个字")]
        public string Detail { get; set; }

        public DateTime VisitTime { get; set; }

        public int? CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        public int? ProjectId { get; set; }
        public virtual Project Project { get; set; }

        [Required(ErrorMessage = "请选择")]
        public int Motivation { get; set; }
        [Required(ErrorMessage = "请选择")]
        public int AreaDemand { get; set; }
        [Required(ErrorMessage = "请选择")]
        public int PriceResponse { get; set; }
        public int CognitiveChannel { get; set; }
        public int Focus { get; set; }
    }

    /// <summary>
    /// 关注重点
    /// </summary>
    [Flags]
    public enum EnumFocus
    {
        [EnumTitle("无", IsDisplay = false)]
        None = 0,

        [EnumTitle("区位")]
        Region = 1,

        [EnumTitle("楼层")]
        Floor = 2,

        [EnumTitle("车位")]
        Garage = 4,

        [EnumTitle("交通")]
        Traffic = 8,

        [EnumTitle("座向")]
        Orientations = 16,

        [EnumTitle("建材")]
        BuildingMaterials  = 32,

        [EnumTitle("工期")]
        TimeLimit = 64,

        [EnumTitle("贷款")]
        Credit = 128,

        [EnumTitle("安全")]
        Security = 256,

        [EnumTitle("学区")]
        SchoolDdistrict = 512,

        [EnumTitle("景观")]
        Sight = 1024,

        [EnumTitle("地段")]
        Sections = 2048,

        [EnumTitle("得房率")]
        ObtainOccupancy= 4096,

        [EnumTitle("价格")]
        Price = 8192,

        [EnumTitle("房型")]
        HouseType = 16384,

        [EnumTitle("面积")]
        Area = 32768,

        [EnumTitle("采光")]
        Daylighting = 65536,

        [EnumTitle("付款方式")]
        PayType = 131072,

        [EnumTitle("品牌信誉")]
        Brand = 262144,

        [EnumTitle("居住品质")]
        Quality = 524288,

        [EnumTitle("物业管理")]
        Property = 1048576,

        [EnumTitle("装修情况")]
        Decoration = 2097152,

        [EnumTitle("建筑风格")]
        ArchitecturalStyle = 4194304,

        [EnumTitle("社区规划及配套")]
        SetAndLayout = 8388608,

        [EnumTitle("周边环境、配套")]
        Peripheral = 16777216,

        [EnumTitle("其他因素")]
        Others = 33554432
    }

    /// <summary>
    /// 认知途径
    /// </summary>
    [Flags]
    public enum EnumCognitiveChannel
    {
        [EnumTitle("无", IsDisplay = false)]
        None = 0,

        [EnumTitle("电视")]
        TV = 1,

        [EnumTitle("短信")]
        Message = 2,

        [EnumTitle("报纸")]
        Paper = 4,

        [EnumTitle("现场")]
        Scene = 8,

        [EnumTitle("房展")]
        HosueExhibition  = 16,

        [EnumTitle("DM直邮")]
        DMMail = 32,

        [EnumTitle("网络")]
        Internet = 64,

        [EnumTitle("杂志")]
        Magazine = 128,

        [EnumTitle("他人介绍")]
        OthersIntroduce = 256,

        [EnumTitle("户外广告")]
        OutdoorAdvertising = 512,

        [EnumTitle("电梯广告")]
        ElevatorAdvertising  = 1024,

        [EnumTitle("夹报")]
        NewspaperClipping  = 2048,

        [EnumTitle("过路客")]
        Passerby = 4096,

        [EnumTitle("其他媒体")]
        Others = 8192
    }

    /// <summary>
    /// 价格反应
    /// </summary>
    public enum EnumPriceResponse
    {
        [EnumTitle("无", IsDisplay = false)]
        None = 0,

        [EnumTitle("较低")]
        Inexpensive = 1,

        [EnumTitle("尚可接受")]
        Acceptable = 2,

        [EnumTitle("较贵")]
        Expensive = 3,

        [EnumTitle("很贵，不能接受")]
        VeryExpensive  = 4
    }

    /// <summary>
    /// 面积需求
    /// </summary>
    public enum EnumAreaDemand
    {
        [EnumTitle("无", IsDisplay = false)]
        None = 0,

        [EnumTitle("200以下")]
        Below200 = 1,

        [EnumTitle("205-210")]
        From205To210 = 2,

        [EnumTitle("210-230")]
        From210To230 = 3,

        [EnumTitle("230以上")]
        Above230 = 4
    }

    /// <summary>
    /// 购房动机
    /// </summary>
    public enum EnumMotivation
    {
        [EnumTitle("无", IsDisplay = false)]
        None = 0,

        [EnumTitle("自住")]
        SelfUse  = 1,

        [EnumTitle("为他人购")]
        PurchaseForOthers = 2,

        [EnumTitle("投资")]
        Investment = 3,

        [EnumTitle("自住投资兼可")]
        SelfUseOrInvestment = 4,

        [EnumTitle("其他")]
        Others = 5
    }

    /// <summary>
    /// 来电方式
    /// </summary>
    public enum EnumVisitWay
    {
        [EnumTitle("来电")]
        Tel = 1,

         [EnumTitle("来访")]
        Visit = 2
    }

    /// <summary>
    /// 跟进级别
    /// </summary>
    public enum EnumFollowLevel
    {
        [EnumTitle("一般")]
        Normal = 1,

        [EnumTitle("进一步")]
        Further = 2,

        [EnumTitle("深入")]
        Thorough = 3
    }

    /// <summary>
    /// 跟进阶段
    /// </summary>
    public enum EnumFollowStep
    {      
        [EnumTitle("意思平平")]
        Insipidity = 1,

        [EnumTitle("有可能性")]
        Potentially = 2,

        [EnumTitle("已确定")]
        Positive = 3
    }

    /// <summary>
    /// 意向产品
    /// </summary>
    public enum EnumProductType
    {
        [EnumTitle("公寓")]
        Apartment = 1,

        [EnumTitle("期房")]
        FuturesHouse = 2,
    }
}
