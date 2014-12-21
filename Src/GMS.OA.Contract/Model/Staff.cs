using System;
using System.Linq;
using GMS.Framework.Contract;
using System.Collections.Generic;
using GMS.Framework.Utility;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GMS.OA.Contract
{
    [Serializable]
    [Table("Staff")]
    public class Staff : ModelBase
    {
        public Staff()
        {
            this.BranchId = 0;
        }

        [StringLength(100)]
        [Required]
        public string Name { get; set; }

        [StringLength(300)]
        public string CoverPicture { get; set; }

        public int Gender { get; set; }

        public int Position { get; set; }

        public DateTime? BirthDate { get; set; }

        [StringLength(50, ErrorMessage = "电话不能超过50个字")]
        public string Tel { get; set; }

        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "电子邮件地址无效")]
        public string Email { get; set; }

        [StringLength(100, ErrorMessage = "地址不能超过100个字")]
        public string Address { get; set; }
      
        public int? BranchId { get; set; }
        public virtual Branch Branch { get; set; }
    }

    /// <summary>
    /// 性别
    /// </summary>
    public enum EnumGender
    {
        [EnumTitle("男")]
        Man = 1,

        [EnumTitle("女")]
        Woman = 2
    }

     /// <summary>
    /// 职位
    /// </summary>
    public enum EnumPosition
    {
        [EnumTitle("无", IsDisplay = false)]
        None = 0,

        [EnumTitle("开发工程师")]
        Development = 1,

        [EnumTitle("高级开发工程师")]
        SDE = 2,

        [EnumTitle("测试工程师")]
        Testing = 3,

        [EnumTitle("项目经理")]
        PM = 4,
    }

}
