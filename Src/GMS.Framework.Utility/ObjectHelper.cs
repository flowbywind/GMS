using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GMS.Framework.Utility
{
    public class ObjectHelper
    {
        /// <summary>
        /// 不同对象之间的深拷贝，最好属性名一样
        /// </summary>
        /// <typeparam name="T">源对象类型</typeparam>
        /// <typeparam name="F">目的对象类型</typeparam>
        /// <param name="original">源对象</param>
        /// <returns>目的对象</returns>
        public static F DeepCopy<T, F>(T original)
        {
            var json = SerializeHelper.JsonSerialize<T>(original);
            var result = SerializeHelper.JsonDeserialize<F>(json);
            return result;
        }

        public static void DeepCopy<T, F>(T original, F desination)
        {
            desination = DeepCopy<T, F>(original);
        }

    }

}
