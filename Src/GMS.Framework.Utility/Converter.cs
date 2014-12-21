using System;
using System.Text;

namespace GMS.Framework.Utility
{
    /// <summary>
    /// 各种值类型转换
    /// </summary>
    public static class Converter
    {
        /// <summary>
        /// 小数转整数，类似四舍五入
        /// </summary>
        /// <param name="value">小数</param>
        /// <returns>整数</returns>
        public static int ToInt(this decimal value)
        {
            var decimalNum = value - (int)value;
            if (decimalNum >= 0.5m)
                return ((int)value) + 1;
            else
                return (int)value;
        }

        /// <summary>
        /// double转整数，类似四舍五入
        /// </summary>
        /// <param name="value">double</param>
        /// <returns>整数</returns>
        public static int ToInt(this double value)
        {
            return ((decimal)value).ToInt();
        }
        
        //public static string ToIntString(this decimal value)
        //{
        //    return value.ToString("f0");
        //}

        //public static string ToIntString(this double value)
        //{
        //    return value.ToString("f0");
        //}

        /// <summary>
        /// 将时间精确到哪个级别
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="cutTicks"></param>
        /// <returns></returns>
        public static DateTime CutOff(this DateTime dateTime, long cutTicks = TimeSpan.TicksPerSecond)
        {
            return new DateTime(dateTime.Ticks - (dateTime.Ticks % cutTicks), dateTime.Kind);
        }
        
        /// <summary>
        /// 把时间转换成字符串如：2013-8-2
        /// </summary>
        /// <param name="dateTime">DateTime</param>
        /// <returns></returns>
        public static string ToCnDataString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// 把时间转换成字符串如：2013-8-2
        /// </summary>
        /// <param name="dateTime">DateTime</param>
        /// <returns></returns>
        public static string ToCnDataString(this DateTime? dateTime)
        {
            return dateTime == null ? string.Empty : dateTime.Value.ToCnDataString();
        }

        /// <summary>
        /// 小数转成价格，如3.123123会转成3.12
        /// </summary>
        /// <param name="price"></param>
        /// <param name="format">小数位数格式</param>
        /// <returns></returns>
        public static string ToPrice(this decimal price, string format = "0.00")
        {
            return price.ToString(format);
        }

        /// <summary>
        /// 价格区间，会转成如 200-300
        /// </summary>
        /// <param name="fromPrice"></param>
        /// <param name="toPrice"></param>
        /// <returns></returns>
        public static string ToShortPriceRange(this decimal fromPrice, decimal toPrice)
        {
            if (fromPrice == toPrice)
                return fromPrice.ToShortPrice();
            else
                return string.Format("{0}-{1}", fromPrice.ToShortPrice(), toPrice.ToShortPrice());
        }

        /// <summary>
        /// 转成价格，如200.45将转成200，小于0时将转成"暂无价格"
        /// </summary>
        /// <param name="price"></param>
        /// <param name="decimalPlaces"></param>
        /// <returns></returns>
        public static string ToShortPrice(this decimal price, int decimalPlaces = 0)
        {
            if (price < 0)
                return "暂无价格";
            return price.ToString("f" + decimalPlaces);
        }

        /// <summary>
        /// 转成价格，如"¥200/晚起"
        /// </summary>
        /// <param name="price"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ToCnDayPrice(this decimal price, string format = "0.00")
        {
            if (price < 0)
                return "暂无报价";

            return string.Format("&yen;{0}/晚起", price.ToString(format));
        }

        /// <summary>
        /// 转成价格，如"¥200"
        /// </summary>
        /// <param name="price"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ToCnPrice(this decimal price, string format = "0.00")
        {
            if (price < 0)
                return "暂无报价";

            return string.Format("&yen;{0}", price.ToString(format));
        }

        /// <summary>
        /// 人名字只留姓，后面用*填充
        /// </summary>
        /// <param name="s"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public static string ToStar(this string s, int start = 1)
        {
            var sb = new StringBuilder();
            if(String.IsNullOrWhiteSpace(s))
            {
                return "*";
            }
            var firstLetter = s[0];
            var firstIsLetter = 65 < firstLetter && firstLetter < 122;
            if (firstIsLetter)
            {
                var array = s.Split(' ');
                if (array.Length > 1 && array[0].Length <= 10)
                {
                    sb.Append(array[0]);
                    if (!String.IsNullOrWhiteSpace(array[1]))
                    {
                        sb.Append(" ");
                        sb.Append(array[1].Substring(0, 1).ToUpper());
                    }
                    else
                        sb.Append("*");
                }
                else
                {
                    var head = array[0];
                    if (head.Length > 10)
                        head = s.Substring(0, 10);
                    sb.Append(head);
                    sb.Append("*");
                }
            }
            else
            {
                var head = s.Substring(0, start);
                sb.Append(head);
                sb.Append("**");
            }

            return sb.ToString();
        }

        /// <summary>
        /// 小数转评分，如3.6转成4，3.3转成3.5，3转成3
        /// </summary>
        /// <param name="score"></param>
        /// <returns></returns>
        public static double ToScore(this double score)
        {
            var decimalNum = score - (int)score;
            if (0 < decimalNum && decimalNum <= 0.5)
                return ((int)score) + 0.5;
            else if (0 < decimalNum && decimalNum > 0.5)
                return ((int)score) + 1;

            return score;
        }

        /// <summary>
        /// 价钱区间转Tuple，如200-300转成Tuple<200, 300>
        /// </summary>
        /// <param name="priceParam"></param>
        /// <returns></returns>
        public static Tuple<int, int> ToPriceRange(this string priceParam)
        {
            if (priceParam.Contains("-"))
            {
                var rangeArray = priceParam.Split('-');
                if (rangeArray.Length == 2)
                {
                    var priceRange = new Tuple<int, int>(rangeArray[0].ToInt(), rangeArray[1].ToInt());
                    return priceRange;
                }
            }

            return new Tuple<int, int>(0, 0);
        }

        /// <summary>
        /// 日期转当前天，跟今天比，如转成“今天”，“昨天”，不符和就转成如“2012-8-2”
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToDay(this DateTime date)
        {
            string s = "";
            var now = DateTime.Now.Day;
            if (now == date.Day)
            {
                s = "今天";
            }
            else if (now - date.Day == 1)
            {
                s = "昨天";
            }
            else
            {
                s = date.ToString("yyyy-MM-dd");
            }
            return s;
        }

        /// <summary>
        /// 日期转星期几，如"星期日", "星期一"
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToWeek(this string date)
        {
            var dayOfWeek = Convert.ToInt32(date.ToDateTime().DayOfWeek);

            string[] weekdays = { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
            return weekdays[dayOfWeek];
        }

        #region Convert string type to other types
        /// <summary>
        /// 字符串转int
        /// </summary>
        /// <param name="s"></param>
        /// <param name="defalut"></param>
        /// <returns></returns>
        public static int ToInt(this string s, int defalut = 0)
        {
            int result = defalut;
            if (int.TryParse(s, out result))
                return result;
            else
                return defalut;
        }

        /// <summary>
        /// 字符串转bool
        /// </summary>
        /// <param name="s"></param>
        /// <param name="defalut"></param>
        /// <returns></returns>
        public static bool ToBool(this string s, bool defalut = false)
        {
            bool result = defalut;
            if (bool.TryParse(s, out result))
                return result;
            else
                return defalut;
        }

        /// <summary>
        /// 字符串转double
        /// </summary>
        /// <param name="s"></param>
        /// <param name="defalut"></param>
        /// <returns></returns>
        public static double ToDouble(this string s, double defalut = 0)
        {
            double result = defalut;
            if (double.TryParse(s, out result))
                return result;
            else
                return defalut;
        }

        /// <summary>
        /// 字符串转decimal
        /// </summary>
        /// <param name="s"></param>
        /// <param name="defalut"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this string s, decimal defalut = 0)
        {
            decimal result = defalut;
            if (decimal.TryParse(s, out result))
                return result;
            else
                return defalut;
        }

        /// <summary>
        /// 字符串转GUID
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Guid ToGuid(this string s)
        {
            Guid result = Guid.Empty;
            if (Guid.TryParse(s, out result))
                return result;
            else
                return Guid.Empty;
        }

        /// <summary>
        /// 字符串转日期
        /// </summary>
        /// <param name="s"></param>
        /// <param name="defalut"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string s, DateTime defalut = new DateTime())
        {
            DateTime result = defalut;
            if (DateTime.TryParse(s, out result))
                return result;
            else
                return defalut;
        }

        /// <summary>
        /// 字符串转Enum
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this string s) where T : struct
        {
            T result = default(T);
            Enum.TryParse<T>(s, true, out result);
            return result;
        }
        #endregion
    }
}
