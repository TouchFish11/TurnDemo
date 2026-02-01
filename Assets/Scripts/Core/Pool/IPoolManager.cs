using System;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using UnityEngine;

namespace Core.Pool
{
    /// <summary>
    /// 对象池管理器接口
    /// </summary>
    public interface IPoolManager
    {
        /// <summary>
        /// 异步获取来自AB包的缓存对象
        /// </summary>
        /// <param name="assetBundleType">AB包类型</param>
        /// <param name="assetName">资源名称</param>
        /// <returns></returns>
        Task<GameObject> GetAssetBundleObjAsync(EAssetBundleType assetBundleType, string assetName);
        
        /// <summary>
        /// 获取未继承Mono的对象
        /// </summary>
        /// <typeparam name="T">类名</typeparam>
        /// <param name="nameSpace">可选参数：命名空间</param>
        /// <returns></returns>
        T GetData<T>(string nameSpace = "") where T : class, IPoolData, new();
        
        /// <summary>
        /// 获取非AB包中的缓存对象
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="assetName">资源名称</param>
        /// <returns></returns>
        T GetObj<T>(string assetName) where T : Behaviour;
        
        /// <summary>
        /// 缓存未继承Mono的对象
        /// </summary>
        /// <typeparam name="T">类名</typeparam>
        /// <param name="data">数据对象</param>
        /// <param name="nameSpace">可选参数：命名空间</param>
        void PushData<T>(T data, string nameSpace = "") where T : class, IPoolData, new();
        
        /// <summary>
        /// 缓存继承Mono的对象
        /// </summary>
        /// <param name="obj">游戏对象</param>
        void PushObj(GameObject obj);

        /// <summary>
        /// 清理指定类型的缓存
        /// </summary>
        /// <typeparam name="T">对象类型，与对象名称一致</typeparam>
        void ClearType<T>();
        
        /// <summary>
        /// 清除指定类型缓存
        /// </summary>
        /// <param name="types"></param>
        void ClearTypes(params Type[] types);
        
        /// <summary>
        /// 清空缓存池
        /// </summary>
        void Clear();
    }
}
