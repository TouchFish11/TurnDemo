#if DISABLE_ADDRESSABLES

#else
using Framework.Singleton;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Framework.Service;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Framework.Addressable
{
    /// <summary>
    /// Addressables异步更新器
    /// </summary>
    [Obsolete("还未完善，建议使用同步更新器 AddressablesUpdater", true)]
    public class AddressablesAsyncUpdater : SingletonBase<AddressablesUpdater>, IAddressablesAsyncUpdater
    {
        // 更新的资源键
        private readonly List<object> _newKeys = new List<object>();
        // 保存旧的定位器Keys，更新前先记录
        private HashSet<object> _oldLocatorKeys = new HashSet<object>();
        // 更新状态
        private EUpdateState _updateState = EUpdateState.None;
        // 更新协程
        private Coroutine _updateCoroutine;
        
        public async Task CheckUpdateAsync(Action<UpdateCallbackData> callback)
        {
            // 避免重复调用
            if (_updateState != EUpdateState.None)
            {
                return;
            }
            
            _updateState =  EUpdateState.Checking;
            // 开始检查资源目录更新
            callback?.Invoke(new UpdateCallbackData(_updateState, 0,0, null));
            // 得到要更新的目录句柄
            var checkHandle = Addressables.CheckForCatalogUpdates();
            // 等待要更新的目录
            await checkHandle.Task;
            // 如果大于0，说明有目录要更新
            if (checkHandle.Result.Count <= 0)
            {
                _updateState = EUpdateState.CheckSuccess;
                callback?.Invoke(new UpdateCallbackData(_updateState, 0, 0, null));
                return;
            }

            // 保存更新前的Keys
            GetPreUpdateKeys();

            // 更新目录
            var updateHandle = Addressables.UpdateCatalogs(checkHandle.Result);
            // 监听目录更新进度
            _updateCoroutine = ServiceLocator.Get<IMonoManager>().StartCoroutine(Progress_Cor(updateHandle, callback));
            // 等待目录更新
            await updateHandle.Task;

            if (updateHandle.Status == AsyncOperationStatus.Succeeded)
            {
                _updateState = EUpdateState.CheckSuccess;
                CompareKeys(updateHandle.Result);
            }
            else
            {
                _updateState = EUpdateState.CheckFailed;
            }
            
            Addressables.Release(updateHandle);
            callback?.Invoke(new UpdateCallbackData(_updateState, 0,0, null));
        }

        public async Task UpdateAssetsAsync(Action<UpdateCallbackData> callback)
        {
            // 先获取下载包的大小
            List<object> keys = await CheckDownloadKey();

            if (keys.Count <= 0)
            {
                _updateState = EUpdateState.UpdateSuccess;
                callback?.Invoke(new UpdateCallbackData(_updateState, 0, 0, null));
                return;
            }

            // 下载AB包
            var downHandle = Addressables.DownloadDependenciesAsync((IEnumerable)keys, Addressables.MergeMode.Union);
            // 监听目录更新进度
            _updateCoroutine = ServiceLocator.Get<IMonoManager>().StartCoroutine(Progress_Cor(downHandle, callback));
            // 等待下载完成
            await downHandle.Task;
            // 更新状态
            _updateState = downHandle.Status == AsyncOperationStatus.Succeeded ? EUpdateState.UpdateSuccess : EUpdateState.UpdateFailed;
            // 执行回调
            callback?.Invoke(new UpdateCallbackData(_updateState, downHandle.GetDownloadStatus().DownloadedBytes,downHandle.GetDownloadStatus().TotalBytes, null));
        }

        /// <summary>
        /// 未完全实现
        /// </summary>
        public void StopUpdate()
        {
            if (_updateCoroutine == null)
            {
                return;
            }
            
            ServiceLocator.Get<IMonoManager>().StopCoroutine(_updateCoroutine); 
            _newKeys.Clear();
        }
        
        void IAddressablesUpdater.CheckUpdate(Action<UpdateCallbackData> callback)
        {
            
        }
        
        void IAddressablesUpdater.UpdateAssets(Action<UpdateCallbackData> callback)
        {
            
        }
        
        /// <summary>
        /// 进度协程
        /// </summary>
        /// <param name="progressHandle"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        private IEnumerator Progress_Cor( AsyncOperationHandle progressHandle, Action<UpdateCallbackData> callback)
        {
            // 通过返回值得到是否更新完毕
            while (!progressHandle.IsDone)
            {
                var downloadStatus = progressHandle.GetDownloadStatus();
                var updateCallbackData = new UpdateCallbackData(_updateState, downloadStatus.DownloadedBytes, downloadStatus.TotalBytes, progressHandle.OperationException);
                callback?.Invoke(updateCallbackData);
                yield return null;
            }
        }

        /// <summary>
        /// 检查下载key
        /// </summary>
        /// <returns></returns>
        private async Task<List<object>> CheckDownloadKey()
        {
            // 判断增量Key是否需要下载（排除无需下载的资源）
            List<object> realNeedDownloadKeys = new List<object>();
            foreach (var key in _newKeys)
            {
                var sizeHandle = Addressables.GetDownloadSizeAsync(key);
                await sizeHandle.Task;

                if (sizeHandle.Status == AsyncOperationStatus.Succeeded && sizeHandle.Result > 0)
                {
                    // 有下载大小，说明需要下载更新
                    realNeedDownloadKeys.Add(key);
                }
            }
            return realNeedDownloadKeys;
        }

        /// <summary>
        /// 对比资源键
        /// </summary>
        /// <param name="locators"></param>
        private void CompareKeys(IList<IResourceLocator> newLocators)
        {
            _newKeys.Clear();
            foreach (var newLocator in newLocators)
            {
                foreach (var newKey in newLocator.Keys)
                {
                    if (newKey == null)
                    {
                        continue;
                    }

                    if (!_oldLocatorKeys.Contains(newKey))
                    {
                        _newKeys.Add(newKey);
                        LogManager.Log($"新增资源keyName：{newKey}");
                    }
                }
            }
        }

        /// <summary>
        /// 获取更新前的Keys
        /// </summary>
        private void GetPreUpdateKeys()
        {
            _oldLocatorKeys.Clear();
            foreach (var locator in Addressables.ResourceLocators)
            {
                foreach (var key in locator.Keys)
                {
                    if (key != null)
                    {
                        _oldLocatorKeys.Add(key);
                    }
                }
            }
        }
    }
}
#endif


