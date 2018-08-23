using Memcached.ClientLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Rock.Library.Helper.Cache
{
    public class MemcachedCache : ICache
    {
        private MemcachedClient memcache = new MemcachedClient();
        /// <summary>
        /// 连接池名称
        /// </summary>
        private string poolName = Config.MemcachedPoolName;
        /// <summary>
        /// 服务器列表
        /// </summary>
        private string[] servers = Config.MemcachedServers;
        /// <summary>
        /// 缓存时常
        /// </summary>
        private int expires = Config.CacheExpires;
        /// <summary>
        /// 获取缓存
        /// </summary>
        public object Get(string key)
        {
            SockIOPool pool = GetPool();
            memcache.PoolName = poolName;
            memcache.EnableCompression = true;
            var value = memcache.Get(key);
            return value;
        }
        /// <summary>
        /// 异步设置缓存
        /// </summary>
        public Task<bool> SetAsync(string key, object value)
        {
            return Task.Factory.StartNew(() =>
            {
                return SetCache(key, value);
            });
        }
        /// <summary>
        /// 设置缓存
        /// </summary>
        public bool Set(string key, object value)
        {
            return SetCache(key, value);
        }
        /// <summary>
        /// 更新缓存
        /// </summary>
        public void Update(string key, object value)
        {
            UpdateCache(key, value);
        }
        /// <summary>
        /// 异步更新缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Task UpdateAsync(string key, object value)
        {
            return Task.Factory.StartNew(() =>
            {
                UpdateCache(key, value);
            });
        }
        /// <summary>
        /// 更新缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        private void UpdateCache(string key, object value)
        {
            SockIOPool pool = GetPool();
            memcache.PoolName = poolName;
            memcache.EnableCompression = false;//是否压缩
            memcache.Replace(key, value, DateTime.Now.AddMinutes(expires));
        }
        /// <summary>
        /// 设置缓存
        /// </summary>
        private bool SetCache(string key, object value)
        {
            try
            {
                SockIOPool pool = GetPool();
                memcache.PoolName = poolName;
                memcache.EnableCompression = true;
                var flag = memcache.Set(key, value, DateTime.Now.AddMinutes(expires));
                return flag;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return false;
            }
        }
        /// <summary>
        /// 获取连接池
        /// </summary>
        /// <returns></returns>
        private SockIOPool GetPool()
        {
            SockIOPool pool = SockIOPool.GetInstance(poolName);
            pool.SetServers(servers);
            pool.InitConnections = 20;//设置开始时每个cache服务器的可用连接数 
            pool.MinConnections = 1;//设置每个服务器最少可用连接数 
            pool.MaxConnections = 1000;//设置每个服务器最大可用连接数 
            pool.SocketConnectTimeout = 10000;//
            pool.SocketTimeout = 10000;
            pool.MaintenanceSleep = 30;
            pool.Failover = true;//设置容错开关
            pool.Nagle = false;//设置是否使用Nagle算法，因为我们的通讯数据量通常都比较大（相对TCP控制数据）而且要求响应及时，因此该值需要设置为false（默认是true）  
            if (!pool.Initialized)
            {
                pool.Initialize();//容器初始化
            }
            return pool;
        }
        /// <summary>
        /// 删除缓存
        /// </summary>
        public void Remove(string key)
        {
            try
            {
                SockIOPool pool = GetPool();
                memcache.PoolName = poolName;
                memcache.EnableCompression = true;
                memcache.Delete(key);
            }
            catch
            {
                throw;
            }
        }
    }
}
