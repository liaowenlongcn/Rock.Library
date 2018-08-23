using Rock.Library.Model;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Rock.Library
{
    public class Utils
    {
        public static bool IsMatched(string fromValue, string toValue)
        {
            if (string.IsNullOrEmpty(fromValue))
                return true;
            return Equals(fromValue, toValue);
        }
        public static string GetGuid(string suffix = "")
        {
            return Guid.NewGuid().ToString() + suffix;
        }
        public static bool IsGuid(string guid)
        {
            if (!string.IsNullOrEmpty(guid))
            {
                var value = Guid.NewGuid();
                Guid.TryParse(guid, out value);
                var flag = guid.Replace("-", "").ToLower() == value.ToString().Replace("-", "").ToLower();
                return flag;
            }
            return false;
        }
        public static Guid ConvertToGuid(string value)
        {
            var guid = Guid.NewGuid();
            Guid.TryParse(value, out guid);
            return guid;
        }
        public static string GetNewId(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;
            return ConvertToGuid(Md5By32(value)).ToString();
        }
        public static string GetGuidFormat(string guid, GuidType guidType)
        {
            string type = guidType.ToString();
            if (string.IsNullOrEmpty(guid))
                return Guid.NewGuid().ToString(type);
            else
                return ConvertToGuid(guid).ToString(type);
        }
        public static string Md5By32(string text)
        {
            return MD5By32(text, Encoding.UTF8);
        }
        public static string MD5ToGUIDString(string value, string valueBase)
        {
            if (String.IsNullOrEmpty(value) || String.IsNullOrEmpty(valueBase))
            {
                return valueBase;
            }
            else
            {
                return ConvertToGuid(Md5By32(value + valueBase)).ToString();
            }
        }
        public static string MD5By32(string text, Encoding encoding)
        {
            //如果字符串为空，则返回
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            try
            {
                //创建MD5密码服务提供程序
                var md5 = new MD5CryptoServiceProvider();

                //计算传入的字节数组的哈希值
                byte[] hashCode = md5.ComputeHash(encoding.GetBytes(text));

                //释放资源
                md5.Clear();

                //返回MD5值的字符串表示
                string temp = "";
                int len = hashCode.Length;
                for (int i = 0; i < len; i++)
                {
                    temp += hashCode[i].ToString("x").PadLeft(2, '0');
                }
                return temp;
            }
            catch
            {
                return string.Empty;
            }
        }
        public static bool IsNumeric(string value)
        {
            try
            {
                if (!string.IsNullOrEmpty(value))
                {
                    return Regex.IsMatch(value, @"^[+-]?\d*[.]?\d*$");
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static string GetEmptyString(int v)
        {
            string eString = "";
            for (int i = 0; i < v; i++)
            {
                eString += " ";
            }
            return eString;
        }
        public static string ValueToFormat(string value)
        {
            var isNumeric = IsNumeric(value);
            if (!isNumeric)
            {
                return "$" + value.Length;
            }
            else
            {
                var values = value.Split('.');
                if (values.Length > 1)
                {
                    return values[0].Length + values[1].Length + "." + values[1].Length;
                }
                else
                {
                    return values[0].Length.ToString();
                }
            }
        }

        public static string IsNullOrZero(string value)
        {
            if (string.IsNullOrEmpty(value) || value == "0")
            {
                return "0";
            }
            else
            {
                return "1";
            }
        }
        public static string GetValue(string value, string defaultValue)
        {
            return string.IsNullOrEmpty(value) ? defaultValue : value;
        }

        public static string GetProtocol(string url, string defaultValue = "http://")
        {
            if (url.ToLower().StartsWith("https"))
            {
                defaultValue = "https://";
            }
            return defaultValue;
        }

        public static MatchCollection MatcheRegex(string value, string regex)
        {
            return Regex.Matches(value, regex);
        }
    }
}
