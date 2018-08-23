using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Rock.Library
{
    public class Config
    {
        #region Memcached设置
        #region Memcached服务器列表
        private static string[] _memcachedServers;
        /// <summary>
        /// Memcached服务器列表
        /// </summary>
        public static string[] MemcachedServers
        {
            get
            {
                try
                {
                    _memcachedServers = _memcachedServers ?? WebConfigurationManager.AppSettings["Memcached_ServerList"].Split(';');
                    return _memcachedServers;
                }
                catch
                {
                    return null;
                }
            }
        }
        #endregion

        #region Memcached连接池名称
        private static string _memcachedPoolName;
        /// <summary>
        /// Memcached连接池名称
        /// </summary>
        public static string MemcachedPoolName
        {
            get
            {
                try
                {
                    _memcachedPoolName = _memcachedPoolName ?? WebConfigurationManager.AppSettings["Memcached_PoolName"].ToString();
                    return _memcachedPoolName;
                }
                catch
                {
                    return null;
                }
            }
        }
        #endregion
        #endregion

        #region 缓存时常
        private static int? _cacheExpires;
        /// <summary>
        /// Memcached缓存时常
        /// </summary>
        public static int CacheExpires
        {
            get
            {
                try
                {
                    _cacheExpires = _cacheExpires ?? Convert.ToInt32(WebConfigurationManager.AppSettings["CacheExpires"].ToString());
                    return Convert.ToInt32(_cacheExpires);
                }
                catch
                {
                    return 20;
                }
            }
        }
        #endregion

        #region 是否调试
        private static bool _isDebug;
        /// <summary>
        /// Memcached缓存时常
        /// </summary>
        public static bool IsDebug
        {
            get
            {
                try
                {
                    _isDebug = WebConfigurationManager.AppSettings["IsDebug"].ToString() == "1" ? true : false;
                    return Convert.ToBoolean(_isDebug);
                }
                catch
                {
                    return false;
                }
            }
        }
        #endregion

        #region 调试用户信息
        private static string _debugUserInfo;
        /// <summary>
        /// Memcached缓存时常
        /// </summary>
        public static string DebugUserInfo
        {
            get
            {
                try
                {
                    _debugUserInfo = WebConfigurationManager.AppSettings["DebugUserInfo"].ToString();
                    return _debugUserInfo;
                }
                catch
                {
                    return "";
                }
            }
        }
        #endregion
    }
}
