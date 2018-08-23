using System.Threading.Tasks;

namespace Rock.Library.Helper.Cache
{
    public interface ICache
    {
        /// <summary>
        /// 异步设置
        /// </summary>
        Task<bool> SetAsync(string key, object value);
        /// <summary>
        /// 设置
        /// </summary>
        bool Set(string key, object value);
        /// <summary>
        /// 设置
        /// </summary>
        object Get(string key);
        /// <summary>
        /// 移除
        /// </summary>
        void Remove(string key);
        /// <summary>
        /// 更新
        /// </summary>
        void Update(string key, object value);
        /// <summary>
        /// 更新缓存
        /// </summary>
        Task UpdateAsync(string key, object value);

    }
}
