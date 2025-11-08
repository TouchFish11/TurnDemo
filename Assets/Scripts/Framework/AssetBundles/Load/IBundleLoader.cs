using System.Collections;
using UnityEngine.Events;

namespace Framework
{
    /// <summary>
    /// 包加载器接口
    /// </summary>
    public interface IBundleLoader
    {
        /// <summary>
        /// 包加载阶段
        /// </summary>
        E_BunldeLoadPhase LoadPhase { get; }

        /// <summary>
        /// 异步加载包
        /// </summary>
        /// <param name="onABLoadprogress"></param>
        /// <returns></returns>
        IEnumerator LoadBundleAsync(UnityAction<float> onABLoadprogress = null);

        /// <summary>
        /// 同步加载包
        /// </summary>
        /// <returns>是否加载成功</returns>
        bool LoadBundle();

        /// <summary>
        /// 同步卸载包
        /// </summary>
        /// <param name="unloadAllLoadedObjects">是否卸载所有已加载的资源</param>
        void Unload(bool unloadAllLoadedObjects = false);

        /// <summary>
        /// 异步卸载包
        /// </summary>
        /// <param name="unloadAllLoadedObjects">是否卸载所有已加载的资源</param>
        /// <returns></returns>
        IEnumerator UnloadAsync(bool unloadAllLoadedObjects = false);

        /// <summary>
        /// 转换加载器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T ConvertTo<T>() where T : class, IBundleLoader;
    }
}
