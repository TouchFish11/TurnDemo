using System;
using Core.AssetBundles.Update.Core;
using Core.DI;
using Core.Log;
using Core.Process;
using Core.UI;
using Core.UI.ViewController;
using Core.Utility;
using HotUpdate.Base.Data;
using HotUpdate.Base.Enums;
using HotUpdate.Base.UI;
using HotUpdate.UI.Tip;

namespace HotUpdate.UI.Begin
{
    using Task = System.Threading.Tasks.Task;
    
    /// <summary>
    /// 开始界面控制器
    /// </summary>
    public class BeginController : UIController<BeginView>
    {
        [Inject] private readonly IAssetBundleUpdater _assetBundleUpdater;
        [Inject] private IUIService _uiService;
        private string _speed;

        protected override bool IsCursorVisible => true;

        /// <summary>
        /// 点击进入游戏事件
        /// </summary>
        public event Func<Task> OnClickEnterGame;

        protected override Task OnInit()
        {
            _assetBundleUpdater.Init();
            ResetState();
            return Task.CompletedTask;
        }

        protected override Task OnActive()
        {
            RegisterUpdateEvent();
            return Task.CompletedTask;
        }

        protected override Task OnInactivate()
        {
            UnRegisterUpdateEvent();
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
            view.SetTextProgress(0);
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
                case EUpdatePhase.DownLoadRemoteCatalogFile:
                    view.SetTextPhase("正在下载资源目录...");
                    break;
                case EUpdatePhase.LoadLocalCatalogFile:
                    view.SetTextPhase("正在读取本地资源目录...");
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
                    // 重置进度条、进度文本
                    view.SetSliderProgress(0);
                    view.SetTextProgress(0);
                    view.SetTextPhase("正在校验资源完整性... (请勿关闭应用程序)");
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
            view.SetTextProgress(currentloadedBytes / (float)totalBytes * 100);
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
            view.SetTextProgress(current / (float)total * 100);
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
            try
            {
                UnRegisterUpdateEvent();
                // 更新成功
                if (updateResult.Success)
                {
                    if (_assetBundleUpdater.GetContext().IsHasUpdate)
                    {
                        var controller = await _uiService.OpenAsync(EUIPanelId.TipPanel, E_UILayer.Mid) as TipController;
                        // 初始化确认数据
                        var confirmData = DIContainer.Create<ConfirmData>();
                        confirmData.ConfirmTitle = "更新提示";
                        confirmData.ConfirmContent = EConfirmContent.AssetUpdate;
                        confirmData.ContentData = "点击确认后将重启游戏";
                        confirmData.ConfirmMessage = "更新成功，请重新启动游戏";
                        confirmData.OnConfirm = ProcessRestarter.RestartProcess;
                        confirmData.OnCancel = null;
                        // 设置提示界面
                        controller.SetTip(confirmData);
                    }
                    else
                    {
                        view.SetUpdateAreaActive(false);
                        view.SetEnterAreaActive(true);
                    }
                }
                // 更新失败
                else
                {
                    // 更新失败
                    var controller = await _uiService.OpenAsync(EUIPanelId.TipPanel, E_UILayer.Mid) as TipController;
                    // 初始化确认数据
                    var confirmData = DIContainer.Create<ConfirmData>();
                    confirmData.ConfirmTitle = "更新提示";
                    confirmData.ConfirmContent = EConfirmContent.AssetUpdate;
                    confirmData.ContentData = "点击确认后将重新下载";
                    confirmData.ConfirmMessage = GetErrorMessage(updateResult.UpdateError);
                    confirmData.OnConfirm = () =>
                    {
                        uiManager.DestroyView(controller.panelId);
                    
                        ResetState();
                        _assetBundleUpdater.Init();
                        RegisterUpdateEvent();
                        _assetBundleUpdater.CheckUpdate();
                    };
                    confirmData.OnCancel = null;
                    
                    // 设置提示界面
                    controller.SetTip(confirmData);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(ELogTags.GameUpdate, $"{nameof(BeginController)}.{nameof(OnUpdateOver)}：{e.Message}，{e.StackTrace}");
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
        
        /// <summary>
        /// 进入主场景
        /// </summary>
        private async void EnterMain()
        {
            try
            {
                // 销毁界面
                await uiManager.DestroyView(panelId);

                if (OnClickEnterGame == null)
                {
                    return;
                }
                
                await OnClickEnterGame.Invoke();
                OnClickEnterGame = null;
            }
            catch (Exception e)
            {
                Logger.LogError(ELogTags.GameUpdate, $"{nameof(BeginController)}.{nameof(EnterMain)}：{e.Message}，{e.StackTrace}");
            }
        }
        
        protected override async void OnButtonClick(string btnName)
        {
            try
            {
                if (btnName == nameof(view.btnStop))
                {
                    await _assetBundleUpdater.UpdateService.CancelDownloadAsync(_assetBundleUpdater.GetContext());
                }
                else if (btnName == nameof(view.btnEnter))
                {
                    EnterMain();
                }
            }
            catch (Exception e)
            {
                Logger.LogError(ELogTags.GameUpdate, $"{nameof(BeginController)}.{nameof(OnButtonClick)}：{e.Message}，{e.StackTrace}");
            }
        }
    }
}