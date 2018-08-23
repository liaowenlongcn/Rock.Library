using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;

namespace Rock.Library.Helper.Cache
{
    public class LocalCache : ICache
    {
        /// <summary>
        /// 缓存时常
        /// </summary>
        private int expires = Config.CacheExpires;
        public Task<bool> SetAsync(string key, object value)
        {
            return Task.Factory.StartNew(() =>
            {
                return SetCache(key, value);
            });
        }
        public bool Set(string key, object value)
        {
            return SetCache(key, value);
        }
        private bool SetCache(string key, object value)
        {
            try
            {
                HttpRuntime.Cache.Insert(key, value, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(expires), CacheItemPriority.High, null);
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return false;
            }
        }
        public object Get(string key)
        {
            return HttpRuntime.Cache.Get(key);
        }
        public void Remove(string key)
        {
            HttpRuntime.Cache.Remove(key);
        }
        public void Update(string key, object o)
        {
            HttpRuntime.Cache.Insert(key, o);
        }
        public Task UpdateAsync(string key, object o)
        {
            return Task.Factory.StartNew(() =>
            {
                HttpRuntime.Cache.Insert(key, o);
            });
        }
    }
}
