using System;
using System.Threading.Tasks;
using Core.AssetBundles.Update;
using Core.AssetBundles.Update.Enum;
using Core.Log;
using Core.Service;
using Core.UI;
using Core.UI.MVC;
using Core.Utility;
using GameHotUpdate.Config;
using GameHotUpdate.Tip.UI.Update;

namespace GameHotUpdate.Update.UI
{
    /// <summary>
    /// 开始界面控制器
    /// </summary>
    public class BeginController : UIController<BeginView, BeginModel>
    {
        private readonly IAssetBundleUpdater _assetBundleUpdater;
        private string _speed;
        
        /// <summary>
        /// 点击进入游戏事件
        /// </summary>
        public event Func<Task> OnClickEnterGame;

        public BeginController()
        {
            _assetBundleUpdater = ServiceLocator.Get<IAssetBundleUpdater>();
        }
        
        protected override Task OnInit()
        {
            _assetBundleUpdater.Init();
            RegisterUpdateEvent();
            ResetState();
            return Task.CompletedTask;
        }

        /// <summary>
        /// 检查更新
        /// </summary>
        public void CheckUpdate()
        {
            _assetBundleUpdater.CheckUpdate();
        }

        /// <summary>
        /// 注册更新事件回调
        /// </summary>
        private void RegisterUpdateEvent()
        {
            _assetBundleUpdater.GetContext().OnUpdatePhase += OnUpdatePhase;
            _assetBundleUpdater.GetContext().OnProgress += OnProgress;
            _assetBundleUpdater.GetContext().OnUpdateSpeed += OnUpdateSpeed;
            _assetBundleUpdater.GetContext().OnCheckProgress += OnCheckProgress;
            _assetBundleUpdater.GetContext().OnUpdateOver += OnUpdateOver;
        }
        
        /// <summary>
        /// 注销更新事件回调
        /// </summary>
        private void UnRegisterUpdateEvent()
        {
            _assetBundleUpdater.GetContext().OnUpdatePhase -= OnUpdatePhase;
            _assetBundleUpdater.GetContext().OnProgress -= OnProgress;
            _assetBundleUpdater.GetContext().OnUpdateSpeed -= OnUpdateSpeed;
            _assetBundleUpdater.GetContext().OnCheckProgress -= OnCheckProgress;
            _assetBundleUpdater.GetContext().OnUpdateOver -= OnUpdateOver;
        }

        /// <summary>
        /// 重置状态
        /// </summary>
        private void ResetState()
        {
            OnUpdatePhase(EUpdatePhase.None);
            view.SetStopButtonActive(false);
            // 先移除
            view.SetUpdateAreaActive(false);
            // 再添加
            view.SetUpdateAreaActive(true);
            view.SetEnterAreaActive(false);
            view.SetSliderProgress(0);
            view.SetTextProgress($"{TextUtility.FloatToStr(0, 2)}%");
            view.SetDownloadSizeAndSpeedText(string.Empty);
        }
        
        /// <summary>
        /// 更新阶段事件回调
        /// </summary>
        /// <param name="updatePhase"></param>
        private void OnUpdatePhase(EUpdatePhase updatePhase)
        {
            switch (updatePhase)
            {
                case EUpdatePhase.None:
                    view.SetTextPhase("正在检查更新...");
                    break;
                case EUpdatePhase.DownLoadRemoteListFile:
                    view.SetTextPhase("正在下载资源清单文件...");
                    break;
                case EUpdatePhase.GetLocalCompareFile:
                    view.SetTextPhase("正在读取本地资源清单文件...");
                    break;
                case EUpdatePhase.CompareContrast:
                    view.SetTextPhase("对比资源差异...");
                    break;
                case EUpdatePhase.DownLoadAssets:
                    view.SetDownloadSizeAndSpeedActive(true);
                    // 显示取消按钮
                    view.SetStopButtonActive(true);
                    view.SetTextPhase("正在下载资源...");
                    break;
                case EUpdatePhase.CheckAssetsIntegrity:
                    // 隐藏取消按钮
                    view.SetStopButtonActive(false);
                    // 隐藏部分UI
                    view.SetDownloadSizeAndSpeedActive(false);
                    view.SetTextPhase("正在校验资源完整性...");
                    break;
                case EUpdatePhase.Finished:
                    view.SetTextPhase("更新完成");
                    break;
            }
        }

        /// <summary>
        /// 下载进度、速度回调
        /// </summary>
        /// <param name="currentloadedBytes"></param>
        /// <param name="totalBytes"></param>
        private void OnProgress(ulong currentloadedBytes, ulong totalBytes)
        {
            var downloadSizeAndSpeed = $"{TextUtility.ToByteUnit(currentloadedBytes)}/{TextUtility.ToByteUnit(totalBytes)} {_speed}";
            view.SetSliderProgress(currentloadedBytes / (float)totalBytes);
            view.SetTextProgress($"{TextUtility.FloatToStr(currentloadedBytes / (float)totalBytes * 100, 2)}%");
            view.SetDownloadSizeAndSpeedText(downloadSizeAndSpeed);
        }

        /// <summary>
        /// 检查完整性进度回调
        /// </summary>
        /// <param name="current"></param>
        /// <param name="total"></param>
        private void OnCheckProgress(int current, int total)
        {
            view.SetSliderProgress(current / (float)total);
            view.SetTextProgress($"{TextUtility.FloatToStr(current / (float)total * 100, 2)}%");
        }

        /// <summary>
        /// 更新速度回调
        /// </summary>
        /// <param name="currentBytes"></param>
        private void OnUpdateSpeed(ulong currentBytes)
        {
            _speed = $"{TextUtility.ToByteUnit(currentBytes)}/s";
        }

        /// <summary>
        /// 更新结束回调
        /// </summary>
        /// <param name="updateResult"></param>
        private async void OnUpdateOver(UpdateResult updateResult)
        {
            UnRegisterUpdateEvent();
            // 更新成功
            if (updateResult.Success)
            {
                view.SetUpdateAreaActive(false);
                view.SetEnterAreaActive(true);
            }
            // 更新失败
            else
            {
                // 更新失败
                var controller = await uiManager.CreateViewAsync<UpdateTipView, UpdateTipModel, UpdateTipController>(AbKeyCollection.Default, E_UILayer.Mid, ResKeyCollection.UpdateTipView);
                // 设置消息
                controller.SetUpdateMessage(GetErrorMessage(updateResult.UpdateError));
                
                // 暂时这样处理，可根据枚举类型决定如何处理按钮点击逻辑
                controller.SetTipActive(true, "点击确认后将重新下载");
                controller.OnSure += () =>
                {
                    uiManager.DestroyView(AbKeyCollection.Default, controller);
                    
                    ResetState();
                    _assetBundleUpdater.Init();
                    RegisterUpdateEvent();
                    _assetBundleUpdater.CheckUpdate();
                };
            }
        }

        /// <summary>
        /// 获取错误消息
        /// </summary>
        /// <param name="updateError"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private static string GetErrorMessage(UpdateResult.EUpdateError updateError)
        {
            return updateError switch
            {
                UpdateResult.EUpdateError.DownloadFailure => "下载错误",
                UpdateResult.EUpdateError.AssetBunleBroken => "资源异常",
                UpdateResult.EUpdateError.LocalListFile => "读取资源文件异常",
                UpdateResult.EUpdateError.AnalyzeAssetBundle => "分析资源差异异常",
                UpdateResult.EUpdateError.DriveStorage => "设备空间不足，请清理后重试",
                UpdateResult.EUpdateError.AssetBunleIncomplete => "资源下载不完整",
                UpdateResult.EUpdateError.Unknown => "未知错误",
                UpdateResult.EUpdateError.None or _ => string.Empty
            };
        }
        
        
        private async void EnterMain()
        {
            // 销毁界面
            uiManager.DestroyView(AbKeyCollection.Default, this);
            // 清空UI管理器
            uiManager.Clear(AbKeyCollection.Default);
            
            await OnClickEnterGame?.Invoke();
            OnClickEnterGame = null;
        }
        
        protected override async void ButtonOnClick(string btnName)
        {
            if (btnName == nameof(view.btnStop))
            {
                await _assetBundleUpdater.GetContext().CancelDownload();
            }
            else if (btnName == nameof(view.btnEnter))
            {
                EnterMain();
            }
        }

        public override void Destroy()
        {
            UnRegisterUpdateEvent();
            base.Destroy();
        }
    }
}