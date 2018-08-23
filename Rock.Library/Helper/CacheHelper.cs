using Rock.Library.Helper.Cache;

namespace Rock.Library.Helper
{
    public class CacheHelper
    {
        private static ICache _MemcachedCache = new LocalCache();//暂时用本地缓存
        /// <summary>
        /// Mc缓存
        /// </summary>
        public static ICache MemcachedCache
        {
            get
            {
                return _MemcachedCache;
            }
        }

        private static ICache _LocalCache = new LocalCache();
        /// <summary>
        /// IIS缓存
        /// </summary>
        public static ICache LocalCache
        {
            get
            {
                return _LocalCache;
            }
        }
    }
}
