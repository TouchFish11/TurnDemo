using Core.AssetBundles.Management;
using Core.AssetBundles.Update;
using Core.AssetBundles.Update.Enum;
using Core.Config;
using Core.Log;
using Core.Service;
using Core.UI;
using Core.Utility;
using GameHotUpdate.UI.MVC;
using GameHotUpdate.UI.Video;

namespace GameHotUpdate.UI.Begin
{
    /// <summary>
    /// ��ʼ���������
    /// </summary>
    public class BeginController : UIController<BeginView, BeginModel>
    {
        protected override System.Threading.Tasks.Task OnInit()
        {
            // ע������¼�
            ServiceLocator.Get<IAssetBundleUpdater>().GetContext().OnUpdatePhase += OnUpdatePhase;
            ServiceLocator.Get<IAssetBundleUpdater>().GetContext().OnProgress += OnProgress;
            ServiceLocator.Get<IAssetBundleUpdater>().GetContext().OnUpdateSpeed += OnUpdateSpeed;
            ServiceLocator.Get<IAssetBundleUpdater>().GetContext().OnCheckProgress += OnCheckProgress;
            ServiceLocator.Get<IAssetBundleUpdater>().GetContext().OnUpdateFinish += OnUpdateFinish;

            OnUpdatePhase(EUpdatePhase.None);
            model.IsActiveProgress = true;
            model.SilderProgress = 0;
            model.TxtProgress = $"{TextUtility.FloatToStr(0, 2)}%";
            model.TxtSize = "";
            model.TxtSpeed = "";
            return System.Threading.Tasks.Task.CompletedTask;
        }

        /// <summary>
        /// ������
        /// </summary>
        public async System.Threading.Tasks.Task CheckUpdate()
        {
            if (!await ServiceLocator.Get<IAssetBundleUpdater>().CheckUpdate())
            {
                LogManager.Log($"����ʧ��");
                return;
            }

            // ��ʼ��AB��
            if (!await ServiceLocator.Get<IAssetBundleManager>().Init())
            {
                LogManager.Log($"AB����ʼ��ʧ��");
                return;
            }

            // ������Ƶ
            PlayVideo();
        }

        public async void PlayVideo()
        {
            VideoController videoController = await ServiceLocator.Get<IUIManager>().CreateViewAsync<VideoView, VideoModel, VideoController>(E_UILayer.Mid, ResKeyCollection.BackView);
            videoController.PlayVideo();
        }

        private void OnUpdatePhase(EUpdatePhase updatePhase)
        {
            switch (updatePhase)
            {
                case EUpdatePhase.None:
                    model.TxtPhase = "���ڼ�����...";
                    break;
                case EUpdatePhase.DownLoadRemoteListFile:
                    model.TxtPhase = "���������嵥�ļ�...";
                    break;
                case EUpdatePhase.GetLocalCompareFile:
                    model.TxtPhase = "���ڶ�ȡ�����嵥�ļ���...";
                    break;
                case EUpdatePhase.CompareContrast:
                    model.TxtPhase = "���ڷ����ļ�����...";
                    break;
                case EUpdatePhase.DownLoadAssets:
                    model.TxtPhase = "����������Դ...";
                    break;
                case EUpdatePhase.CheckAssetsIntegrity:
                    model.TxtPhase = "���ڼ����Դ������...";
                    break;
                case EUpdatePhase.Finished:
                case EUpdatePhase.NullState:
                    model.TxtPhase = "�������";
                    break;
                default:
                    model.TxtPhase = "";
                    LogManager.LogError($"û��ʵ�ָ�ö���{updatePhase}");
                    break;
            }
        }

        private void OnProgress(long currentloadedBytes, long totalBytes)
        {
            model.TxtSize = $"{TextUtility.ToByteUnit((ulong)currentloadedBytes)}/{TextUtility.ToByteUnit((ulong)totalBytes)}";
            model.SilderProgress = currentloadedBytes / (float)totalBytes;
            model.TxtProgress = $"{TextUtility.FloatToStr(model.SilderProgress * 100, 2)}%";
        }

        private void OnCheckProgress(int current, int total)
        {
            model.TxtProgress = $"{TextUtility.FloatToStr((current / (float)total) * 100, 2)}%";
        }

        private void OnUpdateSpeed(long currentBytes)
        {
            model.TxtSpeed = $"{TextUtility.ToByteUnit((ulong)currentBytes)}/s";
        }

        private void OnUpdateFinish()
        {
            // ���ؽ�����
            model.IsActiveProgress = false;
        }
    }
}