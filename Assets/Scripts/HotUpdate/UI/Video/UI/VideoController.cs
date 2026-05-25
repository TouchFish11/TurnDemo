using Core.AssetBundles.Management;
using Core.DI;
using Core.UI.ViewController;
using Core.Video;
using UnityEngine;
using UnityEngine.Video;

namespace HotUpdate.Game.Main.Video.UI
{
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// 视频界面控制器
    /// </summary>
    public class VideoController : UIController<VideoView>
    {
        [Inject] private IVideoManager _videoManager;
        private RenderTexture rawImgVideo;
        
        protected override Task OnInit()
        {
            return Task.CompletedTask;
        }

        protected override Task OnActive()
        {
            return Task.CompletedTask;
        }

        protected override Task OnInactivate()
        {
            return Task.CompletedTask;
        }

        public async void PlayVideo()
        {
            var textureHandle = await GameAsset.LoadAssetAsync<RenderTexture>("VideoTexture");
            var clipHandle = await GameAsset.LoadAssetAsync<VideoClip>("VideoClip");
            rawImgVideo = textureHandle.Asset;
            _videoManager.OnPrePlay += OnPrePlay;
            _videoManager.PlayVideo(clipHandle.Asset, textureHandle.Asset);
        }

        private void OnPrePlay()
        {
            uiManager.DestroyView(panelId);
        }
    }
}
