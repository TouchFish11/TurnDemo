using UnityEngine;

namespace Core.Pool
{
    /// <summary>
    /// 对象池管理器接口
    /// </summary>
    public interface IPoolManager
    {
        /// <summary>
        /// 获取缓存的游戏对象，没有缓存返回null
        /// </summary>
        /// <param name="key">资源Key</param>
        /// <returns></returns>
        T Get<T>(string key) where T : Object;

        /// <summary>
        /// 缓存游戏对象，超过最大容量则直接销毁
        /// </summary>
        /// <param name="obj">游戏对象</param>
        void PushObj<T>(T obj) where T : Object;

        /// <summary>
        /// 缓存纯C#的对象，超过最大容量则不缓存
        /// </summary>
        /// <typeparam name="T">类名</typeparam>
        /// <param name="data">数据对象，为null则不缓存</param>
        void PushData<T>(T data) where T : class, IPoolData;

        /// <summary>
        /// 获取纯C#的对象，自动注入[Inject]依赖，复用对象不会触发构造函数，所以无法通过构造注入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetData<T>() where T : class, IPoolData;
        
        /// <summary>
        /// 释放指定资源缓存，清空指定池子所有对象
        /// </summary>
        /// <param name="assetName">资源名称</param>
        /// <returns>销毁的对象数量</returns>
        int ReleaseCache(string assetName);
        
        /// <summary>
        /// 清空缓存池，清空指定池子所有对象
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
        void ReleaseCache(PoolManager.EReleaseStrategy disposalStrategy = PoolManager.EReleaseStrategy.Trim, ushort executeCount = 1);
    }
}
