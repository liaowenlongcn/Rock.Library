using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Rock.Library
{
    public static class CustomExtension
    {
        #region Json转换
        /// <summary>
        /// 对象转字符串
        /// </summary> 
        public static string ToJson(this object value)
        {
            try
            {
                return JsonConvert.SerializeObject(value);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 字符串转对象
        /// </summary> 
        public static T ToObject<T>(this string value)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
            catch
            {
                return default(T);
            }
        }


        #endregion

        #region 字典转换

        /// <summary>
        /// 将字典类型序列化为json字符串
        /// </summary>
        /// <typeparam name="TKey">字典key</typeparam>
        /// <typeparam name="TValue">字典value</typeparam>
        /// <param name="dic">要序列化的字典数据</param>
        /// <returns>json字符串</returns>
        public static string ToJsonString<TKey, TValue>(this Dictionary<TKey, TValue> dic)
        {
            if (dic.Count == 0)
                return "";

            string jsonStr = JsonConvert.SerializeObject(dic);
            return jsonStr;
        }

        /// <summary>
        /// 将json字符串反序列化为字典类型
        /// </summary>
        /// <typeparam name="TKey">字典key</typeparam>
        /// <typeparam name="TValue">字典value</typeparam>
        /// <param name="jsonStr">json字符串</param>
        /// <returns>字典数据</returns>
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this string jsonStr)
        {
            if (string.IsNullOrEmpty(jsonStr))
                return new Dictionary<TKey, TValue>();

            Dictionary<TKey, TValue> jsonDict = JsonConvert.DeserializeObject<Dictionary<TKey, TValue>>(jsonStr);

            return jsonDict;

        }

        #region 将对象转为字典
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj">对象</param>
        public static Dictionary<String, Object> ToDictionary(this Object obj)
        {
            Dictionary<String, Object> map = new Dictionary<string, object>();
            Type t = obj.GetType();
            PropertyInfo[] pi = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo p in pi)
            {
                MethodInfo mi = p.GetGetMethod();
                if (mi != null && mi.IsPublic)
                {
                    map.Add(p.Name, mi.Invoke(obj, new Object[] { }));
                }
            }
            return map;
        }
        #endregion

        public static Dictionary<string, string> DynamicToDictionary(dynamic obj)
        {
            string s = Convert.ToString(obj);

            string pattern = @"\r*\n";
            Regex rgx = new Regex(pattern);
            s = rgx.Replace(s, "");
            //s = s.Substring(1, s.Length - 1);
            //s = s.Substring(0, s.Length - 1);
            var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(s);
            return dic;
        }


        /// <summary>
        /// 单行表记录转字典
        /// </summary> 
        public static Dictionary<string, string> ToDictionary(this DataRow dr)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach (var column in dr.Table.Columns)
            {
                dic.Add(column.ToString(), dr[column.ToString()].ToString());
            }

            return dic;
        }
        #endregion

        #region 基础类型转换
        public static int ToNumeric(this string value)
        {
            if (Regex.IsMatch(value, @"^[+-]?\d*[.]?\d*$"))
                return Convert.ToInt32(value);
            else
                return 0;
        }


        #endregion

        #region 字符串去除重复空格方法
        /// <summary>
        /// 字符串去除重复空格方法
        /// </summary> 
        public static string DistinctEmptyChar(this string value)
        {
            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex(@"[ ]{2,}", options);
            value = regex.Replace(value, @" ");
            return value;
        }
        #endregion

        #region 字符串转Dictionary

        public static Dictionary<string, string> ToDictionary(this string value)
        {
            try
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                var paras = value.Split('&');
                foreach (var para in paras)
                {
                    var items = para.Split('=');
                    if (items == null || items.Count() < 2) continue;
                    dic.Add(items[0], items[1]);
                }
                return dic;
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}
