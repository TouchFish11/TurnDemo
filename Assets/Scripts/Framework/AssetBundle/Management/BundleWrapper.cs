using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 包包装器
    /// </summary>
    public abstract class BundleWrapper
    {
        // AssetBundle对象
        protected AssetBundle assetBundle;
        // AssetBundle名称
        protected string bundelName;
        // AssetBundle本地加载路径
        protected string loadPath;
        // AB包资源引用计数
        protected uint refCount;
        // AB包加载任务
        protected Task<bool> assetBundleLoadTask;
        // AB包卸载任务
        protected Task<bool> assetBundleUnloadTask;

        /// <summary>
        /// 包装载器
        /// </summary>
        /// <param name="assetBundle"></param>
        public BundleWrapper(string abName, string path)
        {
            this.bundelName = abName;
            this.loadPath = path;
        }

        /// <summary>
        /// 卸载所有已加载的AssetBundle
        /// </summary>
        /// <param name="unloadAllObjects"></param>
        public static void UnloadAllAssetBundles(bool unloadAllObjects)
        {
            AssetBundle.UnloadAllAssetBundles(unloadAllObjects);
        }

        /// <summary>
        /// 异步卸载AssetBundle
        /// </summary>
        /// <param name="unloadAllLoadedObjects"></param>
        /// <returns></returns>
        public virtual Task<bool> UnloadAsync(bool unloadAllLoadedObjects)
        {
            // 正在异步卸载，返回任务
            if (assetBundleUnloadTask != null)
            {
                return assetBundleUnloadTask;
            }

            // 卸载完成，返回true
            if (assetBundle == null)
            {
                return Task.FromResult(true);
            }

            TaskCompletionSource<bool> source = new TaskCompletionSource<bool>();
            assetBundleUnloadTask = source.Task;

            // 异步卸载AB包
            AssetBundleUnloadOperation abuo = assetBundle.UnloadAsync(unloadAllLoadedObjects);
            abuo.completed += (asyncOperation) =>
            {
                source.SetResult(true);
                assetBundle = null;
                LogManager.Log($"{bundelName}包已被卸载");
            };
            return source.Task;
        }

        /// <summary>
        /// 从文件异步加载AssetBundle
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Task<bool> LoadFromFileAsync()
        {
            // 正在异步加载，返回任务
            if (assetBundleLoadTask != null)
            {
                return assetBundleLoadTask;
            }

            // 已加载完成，返回缓存
            if (assetBundle != null)
            {
                return Task.FromResult(true);
            }

            TaskCompletionSource<bool> source = TaskSourceBuilder.CreateTCS<bool>();
            assetBundleLoadTask = source.Task;
            // 异步加载AB包
            AssetBundleCreateRequest abcr = AssetBundle.LoadFromFileAsync(loadPath);
            abcr.completed += (asyncOperation) =>
            {
                this.assetBundle = abcr.assetBundle;
                source.SetResult(this.assetBundle != null);
            };

            return assetBundleLoadTask;
        }

        /// <summary>
        /// 转换为指定类型的包装器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Convert<T>() where T : BundleWrapper
        {
            return this as T;
        }

        /// <summary>
        /// 包名称
        /// </summary>
        public string BundelName => bundelName;

        /// <summary>
        /// 包加载路径
        /// </summary>
        public string LoadPath => loadPath;

        /// <summary>
        /// 包引用数
        /// </summary>
        public abstract uint RefCount { get; }
    }
}
