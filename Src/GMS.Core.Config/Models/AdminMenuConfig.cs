using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace GMS.Core.Config
{
    [Serializable]
    public class AdminMenuConfig : ConfigFileBase
    {
        public AdminMenuConfig()
        {
        }

        public AdminMenuGroup[] AdminMenuGroups { get; set; }
    }

    [Serializable]
    public class AdminMenuGroup
    {
        public List<AdminMenu> AdminMenuArray { get; set; }
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("url")]
        public string Url { get; set; }

        [XmlAttribute("icon")]
        public string Icon { get; set; }

        [XmlAttribute("permission")]
        public string Permission { get; set; }

        [XmlAttribute("info")]
        public string Info { get; set; }
    }

    [Serializable]
    public class AdminMenu
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("url")]
        public string Url { get; set; }

        [XmlAttribute("info")]
        public string Info { get; set; }

        [XmlAttribute("permission")]
        public string Permission { get; set; }
    }
}
