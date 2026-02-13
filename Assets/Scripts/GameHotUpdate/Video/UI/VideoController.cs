using Core.AssetBundles.Management;
using Core.Service;
using Core.Tasks.Extensions;
using Core.UI;
using Core.UI.MVC;
using Core.Video;
using UnityEngine;
using UnityEngine.Video;

namespace GameHotUpdate.Video.UI
{
    /// <summary>
    /// ��Ƶ���������
    /// </summary>
    public class VideoController : UIController<VideoView, VideoModel>
    {
        protected override System.Threading.Tasks.Task OnInit()
        {
            return System.Threading.Tasks.Task.CompletedTask;
        }
        
        public async void PlayVideo()
        {
            var assetBundle = await ServiceLocator.Get<IAssetBundleManager>().LoadBundleAsync(EAssetBundleType.Texture);
            // ������Ⱦ����
            var renderTexture = await assetBundle.LoadAssetAsync<RenderTexture>("VideoTexture").ToTask<RenderTexture>();
            // ��������
            model.RawImgVideo = renderTexture;
            // ������Ƶ
            var videoAb = await ServiceLocator.Get<IAssetBundleManager>().LoadBundleAsync(EAssetBundleType.Video);
            var videoClip = await videoAb.LoadAssetAsync<VideoClip>("").ToTask<VideoClip>();
            // ������Ƶ
            VideoManager.Instance.OnPrePlay += OnPrePlay;
            VideoManager.Instance.PlayVideo(videoClip, renderTexture);
        }

        private void OnPrePlay()
        {
            // ����
            ServiceLocator.Get<IUIManager>().DestroyView(this);
        }
    }
}
