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
    [Table("Article")]
    public class Article : ModelBase
    {
        public Article()
        {
 
        }

        [StringLength(100)]
        [Required]
        public string Title { get; set; }

        [StringLength(300)]
        public string CoverPicture { get; set; }

        [StringLength(int.MaxValue)]
        public string Content { get; set; }

        public int Hits { get; set; }

        public int Diggs { get; set; }

        public bool IsActive { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; }

        public int ChannelId { get; set; }

        public virtual Channel Channel { get; set; }

        public virtual List<Tag> Tags { get; set; }

        [NotMapped]
        public string TagString
        {
            get
            {
                if (Tags != null)
                    return string.Join(",", Tags.Select(t => t.Name));
                else
                    return string.Empty;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    this.Tags = value.Split(',').Select(t => new Tag() { Name = t }).ToList();
                else
                    this.Tags = new List<Tag>();
            }
        }
    }

}
