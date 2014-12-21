using System;
using System.Linq;
using System.Collections.Generic;

namespace GMS.Framework.Utility
{
    public static class Collection
    {
        /// <summary>
        /// 数组或list随机选出几个
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="collection">数组或list</param>
        /// <param name="count">选出数量</param>
        /// <returns></returns>
        public static IEnumerable<T> Random<T>(this IEnumerable<T> collection, int count)
        {
            var rd = new Random();
            return collection.OrderBy(c => rd.Next()).Take(count);
        }

        public static T Random<T>(this IEnumerable<T> collection)
        {
            return collection.Random<T>(1).SingleOrDefault();
        }
    }
}
