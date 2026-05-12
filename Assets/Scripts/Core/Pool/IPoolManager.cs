using UnityEngine;

namespace Core.Pool
{
    /// <summary>
    /// 对象池管理器接口
    /// </summary>
    public interface IPoolManager
    {
        /// <summary>
        /// 获取缓存的游戏对象
        /// </summary>
        /// <param name="key">资源Key</param>
        /// <returns></returns>
        T Get<T>(string key) where T : Object;

        /// <summary>
        /// 缓存游戏对象
        /// </summary>
        /// <param name="obj">游戏对象</param>
        void PushObj(Object obj);

        /// <summary>
        /// 缓存纯C#的对象
        /// </summary>
        /// <typeparam name="T">类名</typeparam>
        /// <param name="data">数据对象</param>
        void PushData<T>(T data) where T : class, IPoolData, new();

        /// <summary>
        /// 获取纯C#的对象，自动注入[Inject]依赖，由于复用对象不会触发构造函数，所以无法通过构造注入
        /// </summary>
        /// <typeparam name="T">类名</typeparam>
        /// <returns></returns>
        T GetData<T>() where T : class, IPoolData, new();
        
        /// <summary>
        /// 清除指定资源缓存
        /// </summary>
        /// <param name="assetName">资源名称</param>
        /// <returns>销毁的对象数量</returns>
        int ClearCache(string assetName);
        
        /// <summary>
        /// 清空缓存池
        /// </summary>
        void ClearAll();

        /// <summary>
        /// 获取指定资源缓存的数量
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        int GetUnUsedCount(string assetName);

        /// <summary>
        /// 强制释放内存，可指定释放的选择策略
        /// </summary>
        /// <param name="disposalStrategy"></param>
        /// <param name="executeCount">执行次数，即释放的池子数量</param>
        void ReleaseCache(PoolManager.EDisposalStrategy disposalStrategy = PoolManager.EDisposalStrategy.Priority, ushort executeCount = 1);
    }
}
