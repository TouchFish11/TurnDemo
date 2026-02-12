using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Log;
using Core.Tasks;
using Core.Tasks.Extensions;
using UnityEngine;

namespace Core.AssetBundles.Management
{
    /// <summary>
    /// 包包装器
    /// </summary>
    public class BundleWrapper
    {
        /// <summary>
        /// AssetBundle对象
        /// </summary>
        public AssetBundle AssetBundle { get; private set; }
        
        /// <summary>
        /// 包名称
        /// </summary>
        public string BundelName { get; }

        /// <summary>
        /// 包加载路径
        /// </summary>
        public string LoadPath { get; }
        
        /// <summary>
        /// 包引用数
        /// </summary>
        public uint RefCount { get; private set; }
        
        /// <summary>
        /// 上次使用的时间
        /// </summary>
        public DateTime LastUseTime { get; private set; }
        
        // AB包加载任务
        private AssetBundleCreateRequestTask _assetBundleCreateRequestTask;
        // AB包卸载任务
        private AssetBundleUnloadOperationTask _assetBundleUnloadTask;

        /// <summary>
        /// 包装载器
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="path"></param>
        public BundleWrapper(string abName, string path)
        {
            BundelName = abName;
            LoadPath = path;
        }

        /// <summary>
        /// 从文件异步加载AssetBundle
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task LoadFromFileAsync(CancellationToken token = default)
        {
            // 正在异步加载，返回任务
            if (_assetBundleCreateRequestTask != null)
            {
                return;
            }
            
            // 已加载完成，直接返回，避免重复加载
            if (AssetBundle)
            {
                RefCount += 1;
                LastUseTime = DateTime.Now;
                LogManager.Log($"{BundelName}包被引用，引用计数更新为：{RefCount}");
                return;
            }
            
            // 异步加载AB包
            _assetBundleCreateRequestTask = AssetBundle.LoadFromFileAsync(LoadPath).ToTask(token);
            AssetBundle = await _assetBundleCreateRequestTask;
            RefCount += 1;
            LastUseTime = DateTime.Now;
            _assetBundleCreateRequestTask = null;
            LogManager.Log($"{BundelName}包被引用，引用计数更新为：{RefCount}");
        }

        /// <summary>
        /// 异步卸载指定AssetBundle
        /// </summary>
        /// <param name="unloadAllLoadedObjects"></param>
        /// <returns></returns>
        public async Task UnloadAsync(bool unloadAllLoadedObjects)
        {
            // 正在异步卸载，返回
            if (_assetBundleUnloadTask != null)
            {
                return;
            }

            // 卸载完成返回
            if (!AssetBundle)
            {
                return;
            }
            
            if (RefCount > 0)
            {
                RefCount -= 1;
            }

            if (RefCount == 0)
            {
                // 异步卸载AB包
                _assetBundleUnloadTask = AssetBundle.UnloadAsync(unloadAllLoadedObjects).ToTask();
                await _assetBundleUnloadTask;
                // 卸载完成后置空
                AssetBundle = null;
                _assetBundleUnloadTask = null;
                LogManager.Log($"{BundelName}包已被卸载，引用计数为：{RefCount}");
            }
            else
            {
                LogManager.Log($"尝试卸载：{BundelName}包，引用计数更新为：{RefCount}");
            }
        }
    }
}
