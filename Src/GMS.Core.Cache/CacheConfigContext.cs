using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.IO;
using GMS.Core.Config;

namespace GMS.Core.Cache
{
    internal class CacheConfigContext
    {
        private static readonly object olock = new object();

        internal static CacheConfig CacheConfig
        {
            get
            {
                return CachedConfigContext.Current.CacheConfig;
            }
        }

        /// <summary>
        /// 首次加载所有的CacheConfig, wrapCacheConfigItem相对于cacheConfigItem把providername通过反射还原成了具体provider类
        /// </summary>
        private static List<WrapCacheConfigItem> wrapCacheConfigItems;
        internal static List<WrapCacheConfigItem> WrapCacheConfigItems
        {
            get
            {
                if (wrapCacheConfigItems == null)
                {
                    lock (olock)
                    {
                        if (wrapCacheConfigItems == null)
                        {
                            wrapCacheConfigItems = new List<WrapCacheConfigItem>();

                            foreach (var i in CacheConfig.CacheConfigItems)
                            {
                                var cacheWrapConfigItem = new WrapCacheConfigItem();
                                cacheWrapConfigItem.CacheConfigItem = i;
                                cacheWrapConfigItem.CacheProviderItem = CacheConfig.CacheProviderItems.SingleOrDefault(c => c.Name == i.ProviderName);
                                cacheWrapConfigItem.CacheProvider = CacheProviders[i.ProviderName];

                                wrapCacheConfigItems.Add(cacheWrapConfigItem);
                            }
                        }
                    }
                }

                return wrapCacheConfigItems;
            }
        }

        /// <summary>
        /// 首次加载所有的CacheProviders
        /// </summary>
        private static Dictionary<string, ICacheProvider> cacheProviders;
        internal static Dictionary<string, ICacheProvider> CacheProviders
        {
            get
            {
                if (cacheProviders == null)
                {
                    lock (olock)
                    {
                        if (cacheProviders == null)
                        {
                            cacheProviders = new Dictionary<string, ICacheProvider>();

                            foreach (var i in CacheConfig.CacheProviderItems)
                                cacheProviders.Add(i.Name, (ICacheProvider)Activator.CreateInstance(Type.GetType(i.Type)));
                        }
                    }
                }

                return cacheProviders;
            }
        }

        /// <summary>
        /// 根据Key，通过正则匹配从WrapCacheConfigItems里帅选出符合的缓存项目，然后通过字典缓存起来
        /// </summary>
        private static Dictionary<string, WrapCacheConfigItem> wrapCacheConfigItemDic;
        internal static WrapCacheConfigItem GetCurrentWrapCacheConfigItem(string key)
        {
            if (wrapCacheConfigItemDic == null)
                wrapCacheConfigItemDic = new Dictionary<string, WrapCacheConfigItem>();

            if (wrapCacheConfigItemDic.ContainsKey(key))
                return wrapCacheConfigItemDic[key];

            var currentWrapCacheConfigItem = WrapCacheConfigItems.Where(i =>
                Regex.IsMatch(ModuleName, i.CacheConfigItem.ModuleRegex, RegexOptions.IgnoreCase) &&
                Regex.IsMatch(key, i.CacheConfigItem.KeyRegex, RegexOptions.IgnoreCase))
                .OrderByDescending(i => i.CacheConfigItem.Priority).FirstOrDefault();

            if (currentWrapCacheConfigItem == null)
                throw new Exception(string.Format("Get Cache '{0}' Config Exception", key));

            lock (olock)
            {
                if (!wrapCacheConfigItemDic.ContainsKey(key))
                    wrapCacheConfigItemDic.Add(key, currentWrapCacheConfigItem);
            }

            return currentWrapCacheConfigItem;
        }

        /// <summary>
        /// 得到网站项目的入口程序模块名名字，用于CacheConfigItem.ModuleRegex
        /// </summary>
        /// <returns></returns>
        private static string moduleName;
        public static string ModuleName
        {
            get
            {
                if (moduleName == null)
                {
                    lock (olock)
                    {
                        if (moduleName == null)
                        {
                            var entryAssembly = Assembly.GetEntryAssembly();

                            if (entryAssembly != null)
                            {
                                moduleName = entryAssembly.FullName;
                            }
                            else
                            {
                                moduleName = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Name;
                            }
                        }
                    }
                }

                return moduleName;
            }
        }

    }

    public class WrapCacheConfigItem
    {
        public CacheConfigItem CacheConfigItem { get; set; }
        public CacheProviderItem CacheProviderItem { get; set; }
        public ICacheProvider CacheProvider { get; set; }
    }
}
