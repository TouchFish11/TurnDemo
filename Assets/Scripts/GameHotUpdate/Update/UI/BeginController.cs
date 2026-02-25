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
        public event Func<Task> OnEnterGame;

        public BeginController()
        {
            _assetBundleUpdater =  ServiceLocator.Get<IAssetBundleUpdater>();
        }
        
        protected override Task OnInit()
        {
            _assetBundleUpdater.GetContext().OnUpdatePhase += OnUpdatePhase;
            _assetBundleUpdater.GetContext().OnProgress += OnProgress;
            _assetBundleUpdater.GetContext().OnUpdateSpeed += OnUpdateSpeed;
            _assetBundleUpdater.GetContext().OnCheckProgress += OnCheckProgress;
            _assetBundleUpdater.GetContext().OnUpdateFinish += OnUpdateFinish;
            _assetBundleUpdater.GetContext().OnUpdateFailResult += OnUpdateFailResult;

            OnUpdatePhase(EUpdatePhase.None);
            view.SetUpdateAreaActive(true);
            view.SetSliderProgress(0);
            view.SetTextProgress($"{TextUtility.FloatToStr(0, 2)}%");
            view.SetDownloadSizeAndSpeedText(string.Empty);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 检查更新
        /// </summary>
        public async Task CheckUpdate()
        {
            await ServiceLocator.Get<IAssetBundleUpdater>().CheckUpdate();
        }

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
                    view.SetTextPhase("正在下载资源...");
                    break;
                case EUpdatePhase.CheckAssetsIntegrity:
                    view.SetTextPhase("正在校验资源完整性...");
                    break;
                case EUpdatePhase.Finished:
                    view.SetTextPhase("更新完成");
                    break;
            }
        }

        private void OnProgress(ulong currentloadedBytes, ulong totalBytes)
        {
            var downloadSizeAndSpeed = $"{TextUtility.ToByteUnit(currentloadedBytes)}/{TextUtility.ToByteUnit(totalBytes)} {_speed}";
            view.SetSliderProgress(currentloadedBytes / (float)totalBytes);
            view.SetTextProgress($"{TextUtility.FloatToStr(currentloadedBytes / (float)totalBytes * 100, 2)}%");
            view.SetDownloadSizeAndSpeedText(downloadSizeAndSpeed);
        }

        private void OnCheckProgress(int current, int total)
        {
            view.SetSliderProgress(current / (float)total);
            view.SetTextProgress($"{TextUtility.FloatToStr(current / (float)total * 100, 2)}%");
        }

        private void OnUpdateSpeed(ulong currentBytes)
        {
            _speed = $"{TextUtility.ToByteUnit(currentBytes)}/s";
        }

        private void OnUpdateFinish()
        {
            LogManager.Log($"下载完成");
            view.SetUpdateAreaActive(false);
            view.SetEnterAreaActive(true);
        }

        private async void OnUpdateFailResult(UpdateResult updateResult)
        {
            // 更新失败，提示
            var controller = await uiManager.CreateViewAsync<UpdateTipView, UpdateTipModel, UpdateTipController>(AbKeyCollection.Default,
                E_UILayer.Mid, ResKeyCollection.UpdateTipView);

            controller.OnSure += () =>
            {
                uiManager.DestroyView(AbKeyCollection.Default, controller);
            };
            // 设置消息
            controller.SetMessage(updateResult.ErrorMessage);
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
            _assetBundleUpdater.GetContext().OnUpdatePhase -= OnUpdatePhase;
            _assetBundleUpdater.GetContext().OnProgress -= OnProgress;
            _assetBundleUpdater.GetContext().OnUpdateSpeed -= OnUpdateSpeed;
            _assetBundleUpdater.GetContext().OnCheckProgress -= OnCheckProgress;
            _assetBundleUpdater.GetContext().OnUpdateFinish -= OnUpdateFinish;
            _assetBundleUpdater.GetContext().OnUpdateFailResult -= OnUpdateFailResult;
            base.Destroy();
        }

        private async void EnterMain()
        {
            // 销毁界面
            uiManager.DestroyView(AbKeyCollection.Default, this);
            // 清空UI管理器
            uiManager.Clear(AbKeyCollection.Default);
            
            await OnEnterGame?.Invoke();
            OnEnterGame = null;
        }
    }
}