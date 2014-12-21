using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace GMS.Core.Config
{
    [Serializable]
    public class CacheConfig : ConfigFileBase
    {
        public CacheConfig()
        {
        }

        public CacheConfigItem[] CacheConfigItems { get; set; }
        public CacheProviderItem[] CacheProviderItems { get; set; }
    }

    public class CacheProviderItem : ConfigNodeBase
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name {get;set;}
        [XmlAttribute(AttributeName = "type")]
        public string Type {get;set;}
        [XmlAttribute(AttributeName = "desc")]
        public string Desc { get; set; }
    }

    public class CacheConfigItem : ConfigNodeBase
    {
        [XmlAttribute(AttributeName = "keyRegex")]
        public string KeyRegex { get; set; }

        [XmlAttribute(AttributeName = "moduleRegex")]
        public string ModuleRegex { get; set; }

        [XmlAttribute(AttributeName = "providerName")]
        public string ProviderName { get; set; }

        [XmlAttribute(AttributeName = "minitus")]
        public int Minitus { get; set; }

        [XmlAttribute(AttributeName = "priority")]
        public int Priority { get; set; }

        [XmlAttribute(AttributeName = "isAbsoluteExpiration")]
        public bool IsAbsoluteExpiration { get; set; }

        [XmlAttribute(AttributeName = "desc")]
        public string Desc { get; set; }
    }
}
