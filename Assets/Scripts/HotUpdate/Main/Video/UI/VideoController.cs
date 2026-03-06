using Core.AssetBundles.Management;
using Core.Service;
using Core.Tasks.Extensions;
using Core.UI;
using Core.UI.MVC;
using Core.Video;
using HotUpdate.Config;
using UnityEngine.Video;

namespace HotUpdate.Main.Video.UI
{
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// 视频界面控制器
    /// </summary>
    public class VideoController : UIController<VideoView, VideoModel>
    {
        protected override Task OnShow()
        {
            return Task.CompletedTask;
        }

        protected override Task OnHide()
        {
            return Task.CompletedTask;
        }

        protected override Task OnInit()
        {
            return Task.CompletedTask;
        }
        
        public async void PlayVideo()
        {
            //var assetBundle = await ServiceLocator.Get<IAssetBundleManager>().LoadBundleAsync(EAssetBundleType.Texture);
            // ������Ⱦ����
            //var renderTexture = await assetBundle.LoadAssetAsync<RenderTexture>("VideoTexture").ToTask<RenderTexture>();
            // ��������
            //model.RawImgVideo = renderTexture;
            // ������Ƶ
            var videoAb = await ServiceLocator.Get<IAssetBundleManager>().LoadBundleAsync("TODO");
            var videoClip = await videoAb.LoadAssetAsync<VideoClip>("").ToTask<VideoClip>();
            // ������Ƶ
            VideoManager.Instance.OnPrePlay += OnPrePlay;
            //VideoManager.Instance.PlayVideo(videoClip, renderTexture);
        }

        private void OnPrePlay()
        {
            // ����
            ServiceLocator.Get<IUIManager>().DestroyView(AbKeyCollection.Ui, this);
        }
    }
}
