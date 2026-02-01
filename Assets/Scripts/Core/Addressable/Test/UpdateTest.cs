#if DISABLE_ADDRESSABLES

#else
using System;
using Framework.Service;
using UnityEngine;

namespace Framework.Addressable.Test
{
    public class UpdateTest : MonoBehaviour
    {
        // Start is called before the first frame update
        private void Start()
        {
            // 初始化框架
            ServiceLocator.InitService();
            
            ServiceLocator.Get<IAddressablesUpdater>().CheckUpdate(CheckUpdateCallback);
        }

        private void CheckUpdateCallback(UpdateCallbackData updateCallbackData)
        {
            switch (updateCallbackData.State)
            {
                case EUpdateState.Checking:
                    LogManager.Log($"正在检查目录更新");
                    break;
                case EUpdateState.CheckSuccess:
                    LogManager.Log($"目录更新成功");
                    // 开始下载资源
                    ServiceLocator.Get<IAddressablesUpdater>().UpdateAssets(UpdateAssetCallback);
                    break;
                case EUpdateState.CheckFailed:
                    LogManager.Log($"目录更新失败，异常：{updateCallbackData.Error}");
                    break;
                case EUpdateState.None:
                case EUpdateState.Updating:
                case EUpdateState.UpdateSuccess:
                case EUpdateState.UpdateFailed:
                default:
                    throw new ArgumentOutOfRangeException($"该方法不应该进入其它阶段，{updateCallbackData.State}");
            }
        }

        private void UpdateAssetCallback(UpdateCallbackData updateCallbackData)
        {
            switch (updateCallbackData.State)
            {
                case EUpdateState.Updating:
                    var currentProgress = updateCallbackData.DownloadedBytes / (float)updateCallbackData.TotalBytes;
                    LogManager.Log($"正在更新资源，当前进度：{currentProgress}");
                    break;
                case EUpdateState.UpdateSuccess:
                    LogManager.Log($"资源更新成功!");
                    break;
                case EUpdateState.UpdateFailed:
                    LogManager.Log($"资源更新失败，异常：{updateCallbackData.Error}");
                    break;
                case EUpdateState.None:
                case EUpdateState.Checking:
                case EUpdateState.CheckSuccess:
                case EUpdateState.CheckFailed:
                default:
                    throw new ArgumentOutOfRangeException($"该方法不应该进入其它阶段，{updateCallbackData.State}");
            }
        }
    }
}
#endif


