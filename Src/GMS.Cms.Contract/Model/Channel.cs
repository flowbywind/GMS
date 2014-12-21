using System;
using System.Linq;
using GMS.Framework.Contract;
using System.Collections.Generic;
using GMS.Framework.Utility;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GMS.Cms.Contract
{
    [Serializable]
    [Table("Channel")]
    public class Channel : ModelBase
    {
        public Channel()
        {
 
        }

        [StringLength(100)]
        [Required]
        public string Name { get; set; }

        [StringLength(300)]
        public string CoverPicture { get; set; }

        [StringLength(300)]
        public string Desc { get; set; }

        public bool IsActive { get; set; }

        public int Hits { get; set; }

        public virtual List<Article> Articles { get; set; }
    }
}
