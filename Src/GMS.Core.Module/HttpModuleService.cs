using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using GMS.Core.Config;
using System.IO;

namespace GMS.Core.Module
{
    /// <summary>
    /// 通用HttpModule，所有网站通过配置注入
    /// </summary>
    public class HttpModuleService : IHttpModule
    {
        private static bool isStarted = false;
        private static object moduleStart = new Object();
        private static Dictionary<string, ContextCollectHandler> handlers;

        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            if (!isStarted)
            {
                lock (moduleStart)
                {
                    if (!isStarted)
                    {
                        isStarted = true;
                        //目前实现主要功能是：通过如"http://localhost/ConfigCollect.axd"访问当前网站的配置，通过"...CacheCollect.axd"访问当前网站的本地缓存
                        //以后可以在这里注入如：消息服务客户端，服务器网站监测数据收集，动态生成图片拦截
                        this.InitHandlers();
                    }
                }
            }

            context.BeginRequest += context_BeginRequest;
        }

        private void InitHandlers()
        {
            if (handlers == null)
                handlers = new Dictionary<string, ContextCollectHandler>();

            var handlerTypes = this.GetType().Assembly.GetTypes().Where(t => t.BaseType == typeof(ContextCollectHandler));

            foreach (var t in handlerTypes)
            {
                if (handlers.ContainsKey(t.Name))
                    continue;

                var instance = Activator.CreateInstance(t) as ContextCollectHandler;
                if (instance != null)
                    handlers.Add(t.Name, instance);
            }
        }

        void context_BeginRequest(object sender, EventArgs e)
        {
            var context = HttpContext.Current;
            var path = context.Request.CurrentExecutionFilePath;

            if (!path.EndsWith("collect.axd", StringComparison.OrdinalIgnoreCase))
                return;

            path = path.Substring(path.LastIndexOf("/") + 1);

            var handler = path.Substring(0, path.LastIndexOf(".")).ToLower();

            if (handlers.ContainsKey(handler))
            {
                handlers[handler].ProcessRequest(context);
            }

        }
    }
}
