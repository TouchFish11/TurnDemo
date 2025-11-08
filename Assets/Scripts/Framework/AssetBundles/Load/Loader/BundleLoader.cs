using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Framework
{
    /// <summary>
    /// 包加载器
    /// </summary>
    public abstract class BundleLoader : IBundleLoader
    {
        //缓存AssetBundle
        protected AssetBundle assetBundle;
        //AssetBundle名称
        protected string bundelName;
        //AssetBundle本地加载路径
        protected string loadPath;
        //AssetBundle加载阶段
        protected E_BunldeLoadPhase loadPhase = E_BunldeLoadPhase.None;
        //AB包资源引用计数
        protected uint refCount;

        public BundleLoader(string abName, string path)
        {
            this.bundelName = abName;
            this.loadPath = path;
            loadPhase = E_BunldeLoadPhase.Start;
        }

        /// <summary>
        /// 异步加载AssetBundle
        /// </summary>
        /// <param name="onABLoadprogress">加载进度回调</param>
        /// <returns></returns>
        public virtual IEnumerator LoadBundleAsync(UnityAction<float> onABLoadprogress = null)
        {
            //加载目标包
            AssetBundleCreateRequest abcr = AssetBundle.LoadFromFileAsync(loadPath);
            loadPhase = E_BunldeLoadPhase.Loading;

            while (!abcr.isDone)
            {
                onABLoadprogress?.Invoke(abcr.progress);
                yield return null;
            }
            onABLoadprogress?.Invoke(abcr.progress);

            if (abcr.assetBundle != null)
            {
                assetBundle = abcr.assetBundle;
                loadPhase = E_BunldeLoadPhase.Finish;
            }
            else
            {
                LogMgr.LogError($"AB包：{bundelName}加载失败，路径{loadPath}");
                loadPhase = E_BunldeLoadPhase.Start;
            }
        }

        /// <summary>
        /// 同步加载AssetBundle
        /// </summary>
        /// <returns></returns>
        public virtual bool LoadBundle()
        {
            //加载目标包
            assetBundle = AssetBundle.LoadFromFile(loadPath);
            if (assetBundle == null)
            {
                LogMgr.LogError($"AB包：{bundelName}加载失败，路径{loadPath}");
                loadPhase = E_BunldeLoadPhase.Start;
                return false;
            }
            loadPhase = E_BunldeLoadPhase.Finish;
            return true;
        }

        /// <summary>
        /// 卸载AB包
        /// </summary>
        public virtual void Unload(bool unloadAllLoadedObjects = false)
        {
            loadPhase = E_BunldeLoadPhase.Start;
            assetBundle.Unload(unloadAllLoadedObjects);
            assetBundle = null;
            LogMgr.Log($"{bundelName}包已被卸载");
        }

        /// <summary>
        /// 异步卸载AB包
        /// </summary>
        public virtual IEnumerator UnloadAsync(bool unloadAllLoadedObjects = false)
        {
            yield return assetBundle.UnloadAsync(unloadAllLoadedObjects);
            loadPhase = E_BunldeLoadPhase.Start;
            LogMgr.Log($"{bundelName}包已被卸载");
            assetBundle = null;
        }

        public T ConvertTo<T>() where T : class, IBundleLoader
        {
            return this as T;
        }

        /// <summary>
        /// 包引用数
        /// </summary>
        public abstract uint RefCount { get; }

        /// <summary>
        /// 资源包加载阶段
        /// </summary>
        public E_BunldeLoadPhase LoadPhase => loadPhase;
    }
}
