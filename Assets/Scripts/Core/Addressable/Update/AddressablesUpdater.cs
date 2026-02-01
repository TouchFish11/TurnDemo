#if DISABLE_ADDRESSABLES

#else
using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Service;
using Framework.Singleton;
using UnityEngine;

namespace Framework.Addressable.Update
{
    /// <summary>
    /// Addressables更新器
    /// </summary>
    public class AddressablesUpdater : SingletonBase<AddressablesUpdater>, IAddressablesUpdater
    {
        // 更新的资源键
        private readonly List<object> _newKeys = new List<object>();
        // 保存旧的定位器Keys，更新前先记录
        private readonly HashSet<object> _oldLocatorKeys = new HashSet<object>();
        // 更新状态
        private EUpdateState  _updateState = EUpdateState.None;
        // 更新协程
        private Coroutine _updateCoroutine;

        private AddressablesUpdater() { }
        
        public void CheckUpdate(Action<UpdateCallbackData> callback)
        {
            // 避免重复调用
            if (_updateState != EUpdateState.None)
            {
                return;
            }
            
            _updateState =  EUpdateState.Checking;
            _updateCoroutine = ServiceLocator.Get<IMonoManager>().StartCoroutine(UpdateCatalogs_Cor(callback));
        }
        
        public void UpdateAssets(Action<UpdateCallbackData> callback)
        {
            if (_newKeys.Count == 0)
            {
                _updateState = EUpdateState.UpdateSuccess;
                callback?.Invoke(new UpdateCallbackData(_updateState, 0,0, null));
                return;
            }

            _updateState = EUpdateState.Updating;
            _updateCoroutine = ServiceLocator.Get<IMonoManager>().StartCoroutine(UpdateAssets_Cor(callback));
        }
        
        public void StopUpdate()
        {
            if (_updateCoroutine == null)
            {
                return;
            }
            
            ServiceLocator.Get<IMonoManager>().StopCoroutine(_updateCoroutine);
            _updateCoroutine = null;
            _updateState = EUpdateState.None;
            _oldLocatorKeys.Clear();
            _newKeys.Clear();
        }
        
        /// <summary>
        /// 更新目录协程
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        private IEnumerator UpdateCatalogs_Cor(Action<UpdateCallbackData> callback)
        {
            // 开始检查资源目录更新
            callback?.Invoke(new UpdateCallbackData(EUpdateState.Checking, 0,0, null));
            // 得到要更新的目录句柄
            var checkHandle = Addressables.CheckForCatalogUpdates();
            // 等待要更新的目录
            yield return checkHandle;
            // 如果大于0，说明有目录要更新
            if (checkHandle.Result.Count <= 0)
            {
                _updateState = EUpdateState.CheckSuccess;
                callback?.Invoke(new UpdateCallbackData(_updateState, 0,0, null));
                yield break;
            }

            // 保存更新前的Keys
            GetPreUpdateKeys();

            // 更新目录
            var updateHandle = Addressables.UpdateCatalogs(checkHandle.Result);
            // 通过返回值得到是否更新完毕
            while (!updateHandle.IsDone)
            {
                var downloadStatus = updateHandle.GetDownloadStatus();
                var updateCallbackData = new UpdateCallbackData(_updateState, downloadStatus.DownloadedBytes, downloadStatus.TotalBytes, updateHandle.OperationException);
                callback?.Invoke(updateCallbackData);
                yield return null;
            }

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
        
        /// <summary>
        /// 更新资源协程
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        private IEnumerator UpdateAssets_Cor(Action<UpdateCallbackData> callback)
        {
            List<object> realNeedDownloadKeys = new List<object>();
            // 先获取下载包的大小
            yield return CheckDownloadKey(realNeedDownloadKeys);

            if (realNeedDownloadKeys.Count <= 0)
            {
                _updateState = EUpdateState.UpdateSuccess;
                callback?.Invoke(new UpdateCallbackData(_updateState,0,0, null));
                yield break;
            }
            
            // 下载AB包
            var downHandle = Addressables.DownloadDependenciesAsync((IEnumerable)realNeedDownloadKeys, Addressables.MergeMode.Union);
            // 进度更新
            DownloadStatus downloadStatus;
            while (!downHandle.IsDone)
            {
                downloadStatus = downHandle.GetDownloadStatus();
                callback?.Invoke(new UpdateCallbackData(_updateState, downloadStatus.DownloadedBytes, downloadStatus.TotalBytes, downHandle.OperationException));
                yield return null;
            }

            downloadStatus = downHandle.GetDownloadStatus();
            callback?.Invoke(downHandle.Status == AsyncOperationStatus.Succeeded
                ? new UpdateCallbackData(_updateState = EUpdateState.UpdateSuccess, downloadStatus.DownloadedBytes, downloadStatus.TotalBytes, null)
                : new UpdateCallbackData(_updateState = EUpdateState.UpdateFailed, downloadStatus.DownloadedBytes, downloadStatus.TotalBytes, null));
        }

        /// <summary>
        /// 检查下载key
        /// </summary>
        /// <returns></returns>
        private IEnumerator CheckDownloadKey(List<object> realNeedDownloadKeys)
        {
            // 判断增量Key是否需要下载（排除无需下载的资源）
            foreach (var key in _newKeys)
            {
                var sizeHandle = Addressables.GetDownloadSizeAsync(key);
                yield return sizeHandle;

                if (sizeHandle.Status == AsyncOperationStatus.Succeeded && sizeHandle.Result > 0)
                {
                    // 有下载大小，说明需要下载更新
                    realNeedDownloadKeys.Add(key);
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
    }
}
#endif



