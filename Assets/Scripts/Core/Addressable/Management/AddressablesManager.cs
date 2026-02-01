#if DISABLE_ADDRESSABLES

#else
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Framework.Singleton;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Framework.Addressable.Management
{
    /// <summary>
    /// Addressables管理器
    /// </summary>
    public class AddressablesManager : SingletonBase<AddressablesManager>, IAddressablesManager
    {
        //存储加载成功的handle
        private readonly Dictionary<string, AddressablesInfo> _resDic = new Dictionary<string, AddressablesInfo>();

        private AddressablesManager()
        {
            
        }
        
        public async Task<AsyncOperationHandle<T>> LoadAssetAsync<T>(string assetName)
        {
            // 自定义key
            var keyName = assetName + "_" + typeof(T).Name;

            AsyncOperationHandle<T> handle;
            if (_resDic.TryGetValue(keyName, out var addressablesInfo))
            {
                handle = addressablesInfo.Handle.Convert<T>();
                // 引用计数加一
                addressablesInfo.RefCount += 1;
                return handle;
            }

            // 第一次异步加载
            handle = Addressables.LoadAssetAsync<T>(assetName);
            // 等待任务完成
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                // 缓存
                _resDic.Add(keyName, new AddressablesInfo(handle));
            }
            else
            {
                LogManager.LogError($"异步加载资源失败，资源名：{assetName}");
            }

            return handle;
        }
        
        public async Task<T> LoadAssetAsync<T>(Addressables.MergeMode mode, params string[] keys)
        {
            // 拼接缓存key
            var list = new List<string>(keys);
            var keyName = "";
            foreach (var key in list)
            {
                keyName += key + "_";
            }
            keyName += typeof(T).Name;
            
            AsyncOperationHandle<T> handle;
            if (_resDic.TryGetValue(keyName, out var addressablesInfo))
            {
                // 引用计数增加
                addressablesInfo.RefCount += 1;
                handle = _resDic[keyName].Handle.Convert<T>();
                return handle.Result;
            }
            
            // 第一次异步加载
            handle = Addressables.LoadAssetAsync<T>(list);
            // 等待任务完成
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                // 缓存
                _resDic.Add(keyName, new AddressablesInfo(handle));
            }
            else
            {
                LogManager.LogError($"异步加载资源失败，资源名：{keyName}");
            }
            
            return  handle.Result;
        }

        public async Task<IList<T>> LoadAssetsAsync<T>(Addressables.MergeMode mergeMode, params string[] keys) where T : Object
        {
            var list = new List<string>(keys);
            var keyName = "";
            foreach (var key in list)
            {
                keyName += key + "_";
            }
            keyName += typeof(T).Name;
            
            AsyncOperationHandle<IList<T>> handle;
            if (_resDic.TryGetValue(keyName, out var addressablesInfo))
            {
                handle = addressablesInfo.Handle.Convert<IList<T>>();
                return handle.Result;
            }
            
            // 第一次异步加载
            handle = Addressables.LoadAssetsAsync<T>(list, null, mergeMode);
            // 等待任务完成
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _resDic.Add(keyName, new AddressablesInfo(handle));
            }
            else
            {
                LogManager.LogError($"异步加载资源失败，资源名：{keyName}");
            }
            return handle.Result;
        }
        
        public void Release<T>(string name)
        {
            var keyName = name + "_" + typeof(T).Name;
            if (!_resDic.TryGetValue(keyName, out var addressablesInfo))
            {
                return;
            }
            
            // 引用计数减一
            addressablesInfo.RefCount -= 1;
            // 如果引用计数为0，才真正卸载资源
            if (_resDic[keyName].RefCount != 0)
            {
                return;
            }
            
            var handle = _resDic[keyName].Handle.Convert<T>();
            // 释放句柄
            Addressables.Release(handle);
            // 移除缓存
            _resDic.Remove(keyName);
        }
        
        public void Release<T>(params string[] keys)
        {
            var list = new List<string>(keys);
            var keyName = "";
            foreach (var key in list)
            {
                keyName += key + "_";
            }
            keyName += typeof(T).Name;

            if (!_resDic.TryGetValue(keyName, out var addressablesInfo))
            {
                return;
            }
            
            // 引用计数减一
            addressablesInfo.RefCount -= 1;
            // 如果引用计数为0，才真正卸载资源
            if (addressablesInfo.RefCount != 0)
            {
                return;
            }
            
            var handle = addressablesInfo.Handle.Convert<T>();
            // 释放句柄
            Addressables.Release(handle);
            // 移除缓存
            _resDic.Remove(keyName);
        }
        
        public void Clear()
        {
            foreach (var item in _resDic.Values)
            {
                Addressables.Release(item.Handle);
            }
            
            _resDic.Clear();
            AssetBundle.UnloadAllAssetBundles(true);
            Resources.UnloadUnusedAssets();
            GC.Collect();
        }
    }
}
#endif



